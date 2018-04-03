using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Rocket.Model
{
    public class PdfOpener: OpenerBase
    {
        public PdfOpener(string filePath, int documentLineNumber, int documentCharacterPosition, int documentLength, int documentPageNumber)
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

        #region OpenerBase

        public override bool Open()
        {
            try {
                Process.Start(FilePath);
                return true;
            } catch(Exception ex) {
                Logger.Error(ex);
            }
            return false;
        }

        #endregion
    }
}
