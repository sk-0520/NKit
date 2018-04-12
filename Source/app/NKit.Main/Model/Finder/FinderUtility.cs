using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model.Searcher;
using ContentTypeTextNet.NKit.Setting.Finder;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public static class FinderUtility
    {
        public static IReadOnlyDictionary<FileNameKind, Regex> CreateFileNameKinds(IReadOnlyFinderSetting setting)
        {
            var patternCreator = new SearchPatternCreator();

            return new[] {
                new { Kind = FileNameKind.Text,  Pattern = setting.TextFileNamePattern, },
                new { Kind = FileNameKind.MicrosoftOffice,  Pattern = setting.MicrosoftOfficeFileNamePattern, },
                new { Kind = FileNameKind.Pdf,  Pattern = setting.PdfFileNamePattern, },
                new { Kind = FileNameKind.XmlHtml,  Pattern = setting.XmlHtmlFileNamePattern, },
            }.Select(i => new {
                Kind = i.Kind,
                Regex = patternCreator.CreateFileNameFilterRegex(i.Pattern)
            }).ToDictionary(
                i => i.Kind,
                i => i.Regex
            );
        }
    }
}
