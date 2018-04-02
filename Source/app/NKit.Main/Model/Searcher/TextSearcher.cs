using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hnx8.ReadJEnc;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Searcher
{
    public class TextSearchMatch
    {
        #region define

        public static TextSearchMatch Unmatch { get; } = new TextSearchMatch(0, 0, 0, string.Empty);

        #endregion

        public TextSearchMatch(int lineNumber, int characterPosition, int length, string lineText)
        {
            if(characterPosition < 0) {
                throw new ArgumentException($"{nameof(characterPosition)}: {characterPosition} < 0");
            }
            if(length < 0) {
                throw new ArgumentException($"{nameof(length)}: {length} < 0");
            }
            if(lineText == null) {
                throw new ArgumentException(nameof(lineText));
            }

            LineNumber = lineNumber;
            CharacterPosition = characterPosition;
            Length = length;
            LineText = lineText;

            DisplayLineNumber = LineNumber;
            DisplayCharacterPosition = CharacterPosition;
        }

        #region property

        public int LineNumber { get; }
        public int CharacterPosition { get; }
        public int Length { get; }
        public string LineText { get; }

        public object Header { get; set; }
        public object Footer { get; set; }

        public string LineUnMatcheHead => LineText.Substring(0, CharacterPosition);
        public string LineUnMatcheTail => LineText.Substring(CharacterPosition + Length);
        public string LineHighlight => LineText.Substring(CharacterPosition, Length);

        public virtual int DisplayLineNumber { get; set; }
        public virtual int DisplayCharacterPosition { get; set; }

        public bool IsMatch => this != Unmatch;

        public object Tag { get; set; }

        #endregion
    }

    public class TextSearchResult : SearchResultBase
    {
        #region define

        public static TextSearchResult NotFound { get; } = new TextSearchResult() {
            IsMatched = false,
        };

        #endregion

        #region property

        public EncodingCheckResult EncodingCheck { get; set; }

        public List<TextSearchMatch> Matches { get; set; } = new List<TextSearchMatch>();

        #endregion
    }

    public class TextSearcher
    {
        #region define

        public static EncodingCheckResult DefaultEncodingCheckResult { get; } = new EncodingCheckResult(true, Encoding.Unicode, string.Empty);

        #endregion

        #region property

        /// <summary>
        /// エンコーディングをチェックする際に用いるデータ長。
        /// <para>この長さに満たない場合は入力データを全て用いる。</para>
        /// <para>エンコーディング情報の取得に失敗した場合は<see cref="DecoderFallbackMaxRetry"/>回、倍々に増やしていく</para>
        /// </summary>
        public int EncodingCheckMaximumLength { get; set; } = 4 * 1024;

        public int DecoderFallbackMaxRetry { get; set; } = 5;

        public ReadJEnc ReadJEnc { get; set; } = ReadJEnc.JP;

        public int ReaderBufferSize { get; } = 4 * 1024; // 何とかしたいなぁ

        #endregion

        #region function

        protected virtual TextSearchMatch CreateMatchObject(int lineNumber, int characterPosition, int length, string lineText)
        {
            return new TextSearchMatch(lineNumber, characterPosition, length, lineText);
        }

        EncodingCheckResult GetEncoding(Stream stream, int checkLength)
        {
            var binary = new byte[checkLength];
            stream.Read(binary, 0, binary.Length);

            var ec = new EncodingChecker();
            var result = ec.GetEncoding(ReadJEnc, binary);

            return result;
        }

        public TextReader CreateReader(Stream stream, Encoding encoding, bool leaveOpen)
        {
            return new StreamReader(stream, encoding, true, ReaderBufferSize, leaveOpen);
        }

        TextSearchResult SearchCore(TextReader reader, Regex regex, EncodingCheckResult encodingCheckResult)
        {
            var result = new TextSearchResult() {
                EncodingCheck = encodingCheckResult,
            };

            foreach(var line in TextUtility.ReadLines(reader).Select((s, i) => new { Value = s, Number = i + 1 })) {
                var macthes = regex.Matches(line.Value).Cast<Match>();
                foreach(var match in macthes) {
                    var searchMatch = CreateMatchObject(line.Number, match.Index, match.Length, line.Value);
                    Debug.Assert(match.Value == searchMatch.LineHighlight, $"{match.Value} != {searchMatch.LineHighlight}");
                    result.Matches.Add(searchMatch);
                }
            }

            result.IsMatched = result.Matches.Any();

            return result;
        }

        public TextSearchResult Search(Stream stream, Regex regex)
        {
            var streamLength = stream.Length;
            var checkLength = (int)Math.Min(EncodingCheckMaximumLength, streamLength);

            foreach(var counter in new Counter(DecoderFallbackMaxRetry)) {
                using(var streamPostionKeeper = new StreamPostionKeeper(stream)) {
                    var encodingCheckResult = GetEncoding(stream, checkLength);
                    if(!encodingCheckResult.IsSuccess) {
                        return new TextSearchResult() {
                            IsMatched = false,
                            EncodingCheck = encodingCheckResult,
                        };
                    }

                    streamPostionKeeper.Reset();
                    try {
                        using(var reader = CreateReader(stream, encodingCheckResult.Encoding, true)) {
                            return SearchCore(reader, regex, encodingCheckResult);
                        }
                    } catch(DecoderFallbackException ex) {
                        Log.Out.Warning(ex);
                        // どう頑張ってもエンコーディング判断が不十分なので死ぬ
                        if(checkLength == streamLength || counter.IsLast) {
                            Log.Out.Warning("unknown encoding");
                            return new TextSearchResult() {
                                IsMatched = false,
                                EncodingCheck = encodingCheckResult,
                            };
                        }
                        var nextCheckLength = counter.IsFirst
                            ? EncodingCheckMaximumLength * 2
                            : checkLength * 2
                        ;
                        checkLength = (int)Math.Min(nextCheckLength, streamLength);
                    }
                }
            }

            // 到達しないはずよ
            return TextSearchResult.NotFound;
        }

        public TextSearchResult Search(string text, Regex regex)
        {
            using(var reader = new StringReader(text)) {
                return SearchCore(reader, regex, DefaultEncodingCheckResult);
            }
        }

        #endregion
    }

    // func<> で定義したら引数名がわかめ
    public delegate TextSearchMatch CustomTextMatchCreatorDelagete(int lineNumber, int characterPosition, int length, string lineText);


    public sealed class CustomTextSearchMatchTextSeacher : TextSearcher
    {
        #region property

        public CustomTextMatchCreatorDelagete CustomTextMatchCreator { get; set; }

        #endregion

        protected override TextSearchMatch CreateMatchObject(int lineNumber, int characterPostion, int length, string lineText)
        {
            if(CustomTextMatchCreator != null) {
                return CustomTextMatchCreator(lineNumber, characterPostion, length, lineText);
            }

            return base.CreateMatchObject(lineNumber, characterPostion, length, lineText);
        }
    }
}
