using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public class HashProvider
    {
        public HashProvider(HashType hashType)
        {
            HashType = hashType;
        }

        #region property

        public HashType HashType { get; }

        #endregion

        #region function

        public byte[] Execute(Stream inputStream)
        {
            var map = new Dictionary<HashType, Func<HashAlgorithm>>() {
                [HashType.SHA1] = () => new SHA1Managed(),
                [HashType.SHA256] = () => new SHA256Managed(),
                [HashType.SHA384] = () => new SHA384Managed(),
                [HashType.SHA512] = () => new SHA512Managed(),
                [HashType.MD5] = () => new MD5Cng(),
            };
            var provider = map[HashType]();

            return provider.ComputeHash(inputStream);
        }

        #endregion

    }
}
