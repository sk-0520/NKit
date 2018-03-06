using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.Library.PInvoke.Windows;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.NKit.Setting.Define;
using ContentTypeTextNet.NKit.NKit.Setting.File;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.File
{
    public class FileOpener
    {
        Process OpenCore(string path)
        {
            try {
                return Process.Start(path);
            } catch(Exception ex) {
                Debug.WriteLine(ex);
            }

            return null;

        }
        public Process Open(FileInfo file)
        {
            return OpenCore(file.FullName);
        }

        public Process Open(DirectoryInfo directory)
        {
            return OpenCore(directory.FullName);
        }

        public Process OpenParentDirectory(FileSystemInfo fileSystemInfo)
        {
            try {
                return Process.Start("explorer", $"/e, /select,{fileSystemInfo.FullName}");
            } catch(Exception ex) {
                Debug.WriteLine(ex);
            }

            return null;
        }

        public void ShowProperty(FileSystemInfo fileSystemInfo, IntPtr hWnd)
        {
            NativeMethods.SHObjectProperties(hWnd, SHOP.SHOP_FILEPATH, fileSystemInfo.FullName, string.Empty);
        }
    }

    public class AssociationSpreadSeetParameter
    {
        #region property
        public string SheetName { get; set; }

        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        #endregion
    }

    public class AssociationDocumentParameter
    {
        #region property

        public int Page { get; set; }

        #endregion
    }

    public class AssociationOpenParameter
    {
        #region property

        public int LineNumber { get; set; }
        public int CharacterPostion { get; set; }
        public int CharacterLength { get; set; }

        public AssociationSpreadSeetParameter SpreadSeet { get; set; } = new AssociationSpreadSeetParameter();

        public AssociationDocumentParameter Document { get; set; } = new AssociationDocumentParameter();

        #endregion
    }

    public class AssociationFileOpener : FileOpener
    {
        public AssociationFileOpener(IReadOnlyAssociationFileSetting setting)
        {
            Setting = setting;
        }

        #region property

        IReadOnlyAssociationFileSetting Setting { get; }

        #endregion

        #region function

        public string FormatArgument(string argument, FileSystemInfo fileSystemInfo, AssociationOpenParameter parameter)
        {
            var map = new Dictionary<string, string>() {
                ["FILE_PATH"] = fileSystemInfo.FullName,
                ["FILE_NAME"] = fileSystemInfo.Name,
                ["LINE"] = parameter.LineNumber.ToString(),
                ["POS"] = parameter.CharacterPostion.ToString(),
                // ------------------
                ["SS_SHEET"] = parameter.SpreadSeet.SheetName,
                ["SS_CELL_X"] = parameter.SpreadSeet.RowIndex.ToString(),
                ["SS_CELL_Y"] = parameter.SpreadSeet.ColumnIndex.ToString(),
                // ------------------
                ["DOC_PAGE"] = parameter.Document.Page.ToString(),
            };
            var to = new TextOperator();

            return to.ReplaceFromDictionary(argument, map);
        }

        Process OpenCore(string path, string arguments)
        {
            try {
                return Process.Start(path, arguments);
            } catch(Exception ex) {
                Debug.WriteLine(ex);
            }

            return null;
        }

        Process OpenTextFile(FileInfo file, AssociationOpenParameter parameter)
        {
            if(!string.IsNullOrEmpty(Setting.TextFileApplicationPath)) {
                var appPath = Environment.ExpandEnvironmentVariables(Setting.TextFileApplicationPath);
                if(global::System.IO.File.Exists(appPath)) {
                    var argument = string.IsNullOrWhiteSpace(Setting.TextFileArgumentFormat)
                        ? file.FullName
                        : FormatArgument(Setting.TextFileArgumentFormat, file, parameter);
                    ;
                    return OpenCore(appPath, argument);
                }
            }

            // 何かしら開けなさそうなら謎ファイルとして処理
            return Open(file);
        }
        Process OpenXmlHtmlFile(FileInfo file, AssociationOpenParameter parameter)
        {
            if(!string.IsNullOrEmpty(Setting.XmlHtmlFileApplicationPath)) {
                var appPath = Environment.ExpandEnvironmentVariables(Setting.XmlHtmlFileApplicationPath);
                if(global::System.IO.File.Exists(appPath)) {
                    var argument = string.IsNullOrWhiteSpace(Setting.XmlHtmlFileArgumentFormat)
                        ? file.FullName
                        : FormatArgument(Setting.XmlHtmlFileArgumentFormat, file, parameter);
                    ;
                    return OpenCore(appPath, argument);
                }
            }

            // 未設定とかファイルがない場合はテキストとして開く
            return OpenTextFile(file, parameter);
        }

        Process OpenMicrosoftOfficeWordFile(FileInfo file, AssociationOpenParameter parameter)
        {
            var processFile = CommonUtility.GetProcessApplication(CommonUtility.GetApplicationDirectoryForApplication());
            var argument = $"--kind {AssociationFileKind.MicrosoftOfficeWord} --path \"{file.FullName}\" --doc_line {parameter.LineNumber} --doc_pos {parameter.CharacterPostion} --doc_len {parameter.CharacterLength} --doc_page {parameter.Document.Page}";
            var executor = new ActionCliApplicationExecutor(processFile.FullName, argument);
            executor.RunAsync(CancellationToken.None).ConfigureAwait(false);
            return executor.ExecuteProcess;
        }

        Process OpenMicrosoftOfficeExcelFile(FileInfo file, AssociationOpenParameter parameter)
        {
            var processFile = CommonUtility.GetProcessApplication(CommonUtility.GetApplicationDirectoryForApplication());
            var argument = $"--kind {AssociationFileKind.MicrosoftOfficeExcel} --path \"{file.FullName}\" --ss_sheet {parameter.SpreadSeet.SheetName} --ss_y {parameter.SpreadSeet.RowIndex} --ss_x {parameter.SpreadSeet.ColumnIndex}";
            var executor = new ActionCliApplicationExecutor(processFile.FullName, argument);
            executor.RunAsync(CancellationToken.None).ConfigureAwait(false);
            return executor.ExecuteProcess;
        }


        public Process Open(AssociationFileKind associationFileKind, FileSystemInfo fileSystemInfo, AssociationOpenParameter parameter)
        {
            switch(associationFileKind) {
                case AssociationFileKind.Unknown:
                    if(fileSystemInfo is FileInfo file) {
                        return Open(file);
                    } else {
                        var dir = fileSystemInfo as DirectoryInfo;
                        Debug.Assert(dir != null);
                        return Open(dir);
                    }

                case AssociationFileKind.Text:
                    return OpenTextFile((FileInfo)fileSystemInfo, parameter);

                case AssociationFileKind.XmlHtml:
                    return OpenXmlHtmlFile((FileInfo)fileSystemInfo, parameter);

                case AssociationFileKind.MicrosoftOfficeExcel:
                    return OpenMicrosoftOfficeExcelFile((FileInfo)fileSystemInfo, parameter);

                case AssociationFileKind.MicrosoftOfficeWord:
                    return OpenMicrosoftOfficeWordFile((FileInfo)fileSystemInfo, parameter);

                default:
                    throw new NotSupportedException();
            }
        }


        #endregion
    }
}
