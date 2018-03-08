using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public class CommandLine
    {
        #region Updater
        [DllImport("shell32.dll", SetLastError = true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), System.Security.SuppressUnmanagedCodeSecurity]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);
        #endregion

        #region static

        /// <summary>
        /// 文字列からコマンドラインパラメータを作成。
        /// <para>http://stackoverflow.com/questions/298830/split-string-containing-command-line-parameters-into-string-in-c-sharp</para>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToCommandLineArguments(string s)
        {
            int argc;
            var argv = CommandLineToArgvW(s, out argc);
            if(argv == IntPtr.Zero) {
                throw new System.ComponentModel.Win32Exception();
            }

            try {
                foreach(var i in Enumerable.Range(0, argc)) {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    yield return Marshal.PtrToStringUni(p);
                }
            } finally {
                Marshal.FreeHGlobal(argv);
            }
        }

        /// <summary>
        /// 文字列から<see cref="CommandLine"/>を作成。
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static CommandLine Create(string s)
        {
            return new CommandLine(ToCommandLineArguments(s));
        }

        #endregion

        /// <summary>
        /// 起動時のオプションから呼び出されることを想定
        /// </summary>
        public CommandLine()
        {
            Options = new List<string>(Environment.GetCommandLineArgs().Skip(1));
        }
        /// <summary>
        /// スタートアップ関数から呼び出されることを想定
        /// </summary>
        /// <param name="args"></param>
        public CommandLine(IEnumerable<string> args)
        {
            Options = args.ToList();
        }

        /// <summary>
        /// オプションヘッダ。
        /// </summary>
        public string KeyValueHeader { get; set; } = "/";
        /// <summary>
        /// オプション分割文字。
        /// </summary>
        public string KeyValueSeparator { get; set; } = "=";

        /// <summary>
        /// 渡されたコマンドラインを統括。
        /// </summary>
        public IReadOnlyList<string> Options { get; private set; }
        /// <summary>
        /// オプション数。
        /// </summary>
        public int Length { get { return Options.Count; } }

        /// <summary>
        /// キー <see cref="KeyValueHeader"/> 値を分割。
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        private KeyValuePair<string, string> SplitKeyValue(string pair)
        {
            var index = pair.IndexOf(KeyValueSeparator, StringComparison.Ordinal);
            if(index != -1) {
                var key = string.Concat(pair.Take(index));
                var value = string.Concat(pair.Skip(index + KeyValueHeader.Length));
                if(value.Length > "\"\"".Length && value.First() == '"' && value.Last() == '"') {
                    value = value.Substring(1, value.Length - 1 - 1);
                }
                return new KeyValuePair<string, string>(key, value);
            }

            throw new ArgumentException(string.Format("pair = {0}, header = {1}", pair, KeyValueHeader));
        }

        /// <summary>
        /// 指定した<see cref="KeyValueHeader"/> + オプション 検索。
        /// </summary>
        /// <param name="keyOption"><see cref="KeyValueHeader"/>を含むキー名。</param>
        /// <returns></returns>
        private IEnumerable<string> Find(string keyOption)
        {
            Debug.Assert(KeyValueHeader == keyOption.Substring(0, KeyValueHeader.Length));

            return Options
                .Where(s => s.Length >= keyOption.Length)
                .Where(s => s.StartsWith(keyOption, StringComparison.Ordinal))
                .Where(s => s == keyOption || s.StartsWith(keyOption + this.KeyValueSeparator, StringComparison.Ordinal))
            ;
        }

        private string KeyToValue(string keyOption, int index)
        {
            Debug.Assert(KeyValueHeader == keyOption.Substring(0, KeyValueHeader.Length));

            var pairs = Options.Where(s => s.StartsWith(keyOption, StringComparison.Ordinal));
            var pair = SplitKeyValue(pairs.ElementAt(index));
            return pair.Value;
        }

        private string GetKeyOption(string option)
        {
            return KeyValueHeader + option;
        }

        private bool HasKeyOption(string keyOption)
        {
            return Options.Any(s => s.StartsWith(keyOption, StringComparison.Ordinal));
        }

        /// <summary>
        /// <see cref="KeyValueHeader"/> + option が存在するかを確認。
        /// <para>データが単独かペアかは問はない。</para>
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public bool HasOption(string option)
        {
            var keyOption = GetKeyOption(option);
            return HasKeyOption(keyOption);
        }

        /// <summary>
        /// 値を持つ <see cref="KeyValueHeader"/> + <paramref name="option"/> が存在するかを確認。
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public bool HasValue(string option)
        {
            return HasValue(option, 0);
        }
        /// <summary>
        /// 値を持つ <see cref="KeyValueHeader"/> + <paramref name="option"/> が存在するかを確認。
        /// </summary>
        /// <param name="option">オプション。</param>
        /// <param name="index">指定オプション内で何番目(0ベース)を対象とするか。</param>
        /// <returns></returns>
        public bool HasValue(string option, int index)
        {
            var keyOption = GetKeyOption(option);
            if(HasKeyOption(keyOption)) {
                var pairs = Options.Where(s => s.StartsWith(keyOption, StringComparison.Ordinal));
                return pairs.ElementAt(index).IndexOf(KeyValueSeparator, StringComparison.Ordinal) != -1;
            }

            return false;
        }

        private int CountKeyOption(string keyOption)
        {
            return Options.Count(s => s.StartsWith(keyOption, StringComparison.Ordinal));
        }

        /// <summary>
        /// オプション数取得。
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public int CountOption(string option)
        {
            var keyOption = GetKeyOption(option);
            return CountKeyOption(keyOption);
        }

        /// <summary>
        /// 値を列挙。
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public IEnumerable<string> GetValues(string option)
        {
            var keyOption = GetKeyOption(option);
            var optionCount = CountKeyOption(keyOption);

            foreach(var i in Enumerable.Range(0, optionCount)) {
                yield return KeyToValue(keyOption, i);
            }
        }

        /// <summary>
        /// 値を取得。
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public string GetValue(string option)
        {
            return GetValues(option).First();
        }
    }
}
