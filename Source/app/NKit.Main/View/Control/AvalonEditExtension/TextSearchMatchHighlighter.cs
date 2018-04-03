using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.Searcher;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ContentTypeTextNet.NKit.Main.View.Control.AvalonEditExtension
{
    public class TextSearchMatchallLinesHighlighter : DocumentColorizingTransformer
    {
        public TextSearchMatchallLinesHighlighter(IReadOnlyList<TextSearchMatch> matches, Brush matchForeground, Brush matchBackground, FontWeight matchFontWeight)
        {
            Matches = matches;

            MatchForeground = matchForeground;
            MatchBackground = matchBackground;
            MatchFontWeight = matchFontWeight;
        }

        #region property

        IReadOnlyList<TextSearchMatch> Matches { get; }
        Brush MatchForeground { get; }
        Brush MatchBackground { get; }
        FontWeight MatchFontWeight { get; }

        #endregion

        protected override void ColorizeLine(DocumentLine line)
        {
            if(Matches.Count <= line.LineNumber - 1) {
                return;
            }

            var match = Matches[line.LineNumber - 1];

            var start = line.Offset + match.CharacterPosition;
            var end = start + match.Length;
            if(line.Offset <= start && end <= line.EndOffset) {
                
                ChangeLinePart(start, end, elm => {
                    var trp= elm.TextRunProperties;
                    trp.SetForegroundBrush(MatchForeground);
                    trp.SetBackgroundBrush(MatchBackground);
                    trp.SetTypeface(new Typeface(trp.Typeface.FontFamily, trp.Typeface.Style, MatchFontWeight, trp.Typeface.Stretch));
                });
            }
        }
    }
}
