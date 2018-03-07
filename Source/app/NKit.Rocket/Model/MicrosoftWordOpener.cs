using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model.Unmanaged;
using Word = Microsoft.Office.Interop.Word;

namespace ContentTypeTextNet.NKit.Rocket.Model
{
    public class MicrosoftWordOpener : ComApplicationOpenerBase
    {
        public MicrosoftWordOpener(string filePath, int documentLineNumber, int documentCharacterPosition, int documentLength, int documentPageNumber)
            : base(filePath)
        {
            DocumentLineNumber = documentLineNumber;
            DocumentCharacterPosition = documentCharacterPosition;
            DocumentLength = documentLength;
            DocumentPageNumber = documentPageNumber;
        }

        #region property
        int DocumentLineNumber { get; }
        int DocumentCharacterPosition { get; }
        int DocumentLength { get; }
        int DocumentPageNumber { get; }

        #endregion

        #region function

        public override bool Open()
        {
            ComModel<Word.Application> word = null;

            try {
                word = new ComModel<Word.Application>(new Word.Application());
                ApplicationQuitAction = () => word.Com.Quit();
            } catch(InvalidCastException ex) {
                Trace.WriteLine(ex);
                // Word が入ってなさげなので通常のファイルオープンでさよなら。
                // シェルから開けないんならこっちの責任じゃない
                // まぁ行指定方法とか分からないんですけどね
                Process.Start(FilePath);
                return false;
            }

            using(word) {
                try {
                    word.Com.Visible = false;

                    using(var documents = ComModel.Create(word.Com.Documents)) {
                        using(var document = ComModel.Create(documents.Com.Open(FilePath))) {
                            try {
                                using(var paragraphs = ComModel.Create(document.Com.Paragraphs)) {
                                    // こっちの持ってるデータも怪しいので追々調整していくべし
                                    using(var paragraph = ComModel.Create(paragraphs.Com[DocumentLineNumber])) {
                                        using(var paraRange = ComModel.Create(paragraph.Com.Range)) {
                                            word.Com.Visible = true;

                                            paraRange.Com.Start += DocumentCharacterPosition;
                                            paraRange.Com.End = paraRange.Com.Start + DocumentLength;
                                            paraRange.Com.Select();

                                            CanApplicationQuit = false;
                                            return true;
                                        }
                                    }
                                }
                            } finally {
                                if(CanApplicationQuit) {
                                    document.Com.Close(false);
                                }
                            }
                        }
                    }
                } finally {
                    QuitAppication();
                }
            }
        }

        #endregion
    }
}
