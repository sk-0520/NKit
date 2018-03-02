using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Forms = System.Windows.Forms;

namespace ContentTypeTextNet.NKit.Utility.CompatibleForms
{
    /// <summary>
    /// FormsのFormをウィンドウとして扱う。
    /// <para>要はウィンドウハンドル欲しい。</para>
    /// </summary>
    public class CompatibleFormWindow : Forms.IWin32Window
    {
        public CompatibleFormWindow(Window window)
        {
            var helper = new WindowInteropHelper(window);
            Handle = helper.Handle;
        }

        #region IWin32Window

        /// <summary>
        /// ウィンドウハンドル。
        /// </summary>
        public IntPtr Handle { get; }

        #endregion
    }
}
