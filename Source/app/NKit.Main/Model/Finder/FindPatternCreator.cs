using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public class FindPatternCreator
    {
        public FindPatternCreator()
        { }


        #region function

        Regex CreatePartialRegex(string pattern, RegexOptions options)
        {
            var escapedPattern = Regex.Escape(pattern);
            return new Regex(escapedPattern, options);
        }

        string CreateWildcardRegexPattern(string wildcard)
        {
            var escapedPattern = Regex.Escape(wildcard);
            var wildcardPattern = escapedPattern.Replace("\\?", ".").Replace("\\*", ".*");

            return wildcardPattern;
        }

        string AddWildcardBothLimit(string s) => "^" + s + "$";

        Regex CreateWildcardRegex(string pattern, RegexOptions options)
        {
            var wildcardPattern = CreateWildcardRegexPattern(pattern);
            var usingPattern = AddWildcardBothLimit(wildcardPattern);

            return new Regex(usingPattern, options);
        }

        public Regex CreateRegex(FindPatternKind findPatternKind, string pattern, bool ignoreCase)
        {
            var options = RegexOptions.None;
            if(ignoreCase) {
                options |= RegexOptions.IgnoreCase;
            }

            var usingPattern = pattern ?? string.Empty;

            switch(findPatternKind) {
                case FindPatternKind.PartialMatch:
                    return CreatePartialRegex(usingPattern, options);

                case FindPatternKind.WildcardCharacter:
                    return CreateWildcardRegex(usingPattern, options);

                case FindPatternKind.Regex:
                    return new Regex(usingPattern, options);

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// ファイル名とファイル内容の簡易紐付け正規表現を構築。
        /// </summary>
        /// <param name="pattern">|で区切られたワイルドカード</param>
        /// <returns></returns>
        public Regex CreateFileNameFilterRegex(string pattern)
        {
            var wildcardNames = pattern
                .Split('|')
                .Select(s => CreateWildcardRegexPattern(s))
                .Select(s => "(" + s + ")")
            ;

            var joinPattern = string.Join("|", wildcardNames);
            var bothLimitPattern = AddWildcardBothLimit(joinPattern);
            return CreateRegex(FindPatternKind.Regex, bothLimitPattern, true);
        }

        #endregion
    }
}
