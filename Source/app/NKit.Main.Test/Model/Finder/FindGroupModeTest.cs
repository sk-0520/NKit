using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Setting.Finder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContentTypeTextNet.NKit.Main.Test.Model.Finder
{
    [TestClass]
    public class FindGroupModeTest
    {
        [TestMethod]
        public void IsMatchFileAttributesTest()
        {

            var model = new PrivateObject(new FindGroupModel(null, null, null, null, null));

            var setting = new FindGroupSetting();
            setting.FindFileProperty = true;

            var cache = new FindGroupCache() {
                Setting = setting,
            };

            model.SetProperty("Cache", cache);

            var method = "IsMatchFileAttributes";

            setting.FilePropertyFileAttributes
                = FileAttributes.Archive
                | FileAttributes.Directory
                | FileAttributes.Normal
                | FileAttributes.SparseFile
            ;

            setting.FilePropertyFileAttributeFlagMatchKind = NKit.Setting.Define.FlagMatchKind.Has;
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.Archive));
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.SparseFile));
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.Normal));
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.Directory));
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.Offline));
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.Archive | FileAttributes.Normal | FileAttributes.Directory));
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.Normal | FileAttributes.Directory));
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.Offline | FileAttributes.Archive | FileAttributes.Normal | FileAttributes.Directory));
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.Offline | FileAttributes.Normal | FileAttributes.Directory));

            setting.FilePropertyFileAttributeFlagMatchKind = NKit.Setting.Define.FlagMatchKind.Approximate;
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.Archive));
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.SparseFile));
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.Normal));
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.Directory));
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.Offline));
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.Archive | FileAttributes.Normal | FileAttributes.Directory | FileAttributes.SparseFile));
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.Archive | FileAttributes.Normal | FileAttributes.SparseFile));
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.Archive | FileAttributes.Directory | FileAttributes.SparseFile));
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.Offline | FileAttributes.Archive | FileAttributes.SparseFile));

            setting.FilePropertyFileAttributeFlagMatchKind = NKit.Setting.Define.FlagMatchKind.Full;
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.Archive | FileAttributes.Normal | FileAttributes.Directory | FileAttributes.SparseFile));
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.Archive | FileAttributes.Normal | FileAttributes.SparseFile));
            Assert.IsTrue((bool)model.Invoke(method, FileAttributes.Archive | FileAttributes.Directory | FileAttributes.SparseFile));
            Assert.IsFalse((bool)model.Invoke(method, FileAttributes.Offline | FileAttributes.Archive | FileAttributes.SparseFile));
        }
    }
}
