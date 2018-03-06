using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model.Unmanaged;
using Word = Microsoft.Office.Interop.Word;

namespace ContentTypeTextNet.NKit.Process.Model
{
    public class MicrosoftWordOpener : ComOpenerBase
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
            using(var word = ComModel.Create(new Word.Application())) {
                try {
                    word.Com.Visible = false;

                    using(var documents = ComModel.Create(word.Com.Documents)) {
                        using(var document = ComModel.Create(documents.Com.Open(FilePath))) {
                            try {
                                using(var paragraphs = ComModel.Create(document.Com.Paragraphs)) {
                                    using(var paragraph = ComModel.Create(paragraphs.Com[DocumentLineNumber + 1])) {
                                        //paragraph.Com.Range
                                        using(var paraRange = ComModel.Create(paragraph.Com.Range)) {
                                            word.Com.Visible = true;

                                            paraRange.Com.Start += DocumentCharacterPosition + 1;
                                            paraRange.Com.End = paraRange.Com.Start + DocumentLength + 1;
                                            paraRange.Com.Select();

                                            ExcelQuit = false;
                                            return true;
                                        }
                                    }
                                }
                            } finally {
                                if(ExcelQuit) {
                                    document.Com.Close(false);
                                }
                            }
                        }
                    }
                } finally {
                    if(ExcelQuit) {
                        word.Com.Quit();
                    }
                }
            }

            #endregion
        }
    }
}
