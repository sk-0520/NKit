using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hnx8.ReadJEnc;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public class EncodingUtility
    {
        #region property

        public static Encoding UTF8n => new UTF8Encoding(false);

        #endregion

        #region function

        public static Encoding Parse(string encodingName)
        {
            if(encodingName == "utf-8n") {
                return UTF8n;
            }

            return Encoding.GetEncoding(encodingName);
        }

        #endregion
    }

    public class EncodingCheckResult
    {
        #region define
        public static EncodingCheckResult Unknown { get; } = new EncodingCheckResult(false, null, null);

        #endregion

        public EncodingCheckResult(bool isSuccess, Encoding encoding, string convertedText)
        {
            IsSuccess = isSuccess;
            Encoding = encoding;
            ConvertedText = convertedText;
        }

        #region property

        public bool IsSuccess { get; }
        public Encoding Encoding { get; }
        public string ConvertedText { get; }

        #endregion

    }

    public class EncodingChecker
    {
        #region property

        #endregion

        #region function

        public EncodingCheckResult GetEncodingFromCurrentCulture(byte[] binary)
        {
            return GetEncodingFromCurrentCulture(binary, binary.Length);
        }

        public EncodingCheckResult GetEncodingFromCurrentCulture(byte[] binary, int length)
        {
            var targetJenc = ReadJEnc.JP;
            return GetEncoding(targetJenc, binary);
        }

        public EncodingCheckResult GetEncoding(ReadJEnc jenc, byte[] binary)
        {
            return GetEncoding(jenc, binary, binary.Length);
        }

        EncodingCheckResult GetEncoding(ReadJEnc jenc, byte[] binary, int length)
        {
            string outputText;
            var charCode = jenc.GetEncoding(binary, length, out outputText);
            if(charCode == null) {
                return EncodingCheckResult.Unknown;
            }

            var encoding = charCode.GetEncoding();

            return new EncodingCheckResult(true, encoding, outputText);
        }

        #endregion
    }
}
