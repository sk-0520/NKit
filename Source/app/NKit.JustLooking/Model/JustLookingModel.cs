using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Browser.Model;
using ContentTypeTextNet.NKit.Utility.Model;
using Microsoft.Extensions.CommandLineUtils;

namespace ContentTypeTextNet.NKit.JustLooking.Model
{
    public class JustLookingModel : ModelBase
    {
        #region property

        public BrowserModel Browser { get; private set; }
        public string FilePath { get; private set; }

        #endregion

        #region function

        public void Initialize(string[] arguments)
        {
            var commandLineApplication = new CommandLineApplication(false);

            var optionFilePath = commandLineApplication.Option("--file_path", "open file", CommandOptionType.SingleValue);
            var optionBrowserKind = commandLineApplication.Option("--browser_kind", "ContentTypeTextNet.NKit.Browser.Model.BrowserKind", CommandOptionType.SingleValue);
            var optionEncoding = commandLineApplication.Option("--encoding", "encoding", CommandOptionType.SingleValue);

            commandLineApplication.Execute(arguments);

            if(!optionFilePath.HasValue()) {
                throw new ArgumentException(optionFilePath.ToString());
            }
            var filePath = optionFilePath.Value();
            if(string.IsNullOrWhiteSpace(filePath)) {
                throw new ArgumentException(optionFilePath.ToString());
            }
            FilePath = filePath;
            var fileInfo = new FileInfo(Environment.ExpandEnvironmentVariables(FilePath));

            if(!optionBrowserKind.HasValue()) {
                throw new ArgumentException(optionBrowserKind.ToString());
            }
            var browserKind = EnumUtility.Parse<BrowserKind>(optionBrowserKind.Value());

            Encoding encoding;
            if(!optionEncoding.HasValue()) {
                encoding = Constants.DefaultEncoding;
            } else {
                encoding = EncodingUtility.Parse(optionEncoding.Value());
            }

            Browser = new BrowserModel(browserKind, fileInfo, encoding);
        }

        public void OpenDirectory()
        {
            try {
                Process.Start("explorer", $"/e, /select,{Browser.FileInfo.FullName}");
            } catch(Exception ex) {
                Logger.Warning(ex);
            }
        }

        public void OpenFile()
        {
            try {
                Process.Start(Browser.FileInfo.FullName);
            } catch(Exception ex) {
                Logger.Warning(ex);
            }
        }

        #endregion
    }
}
