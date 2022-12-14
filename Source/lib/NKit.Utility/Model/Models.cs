using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Define;
using Prism.Mvvm;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public abstract class ModelBase : BindableBase, IDisposable
    {
        public ModelBase()
        {
            Logger = Log.CreateLogger(GetType().Name);
        }

        ~ModelBase()
        {
            Dispose(false);
        }

        #region property

        protected ILogger Logger { get; private set; }

        #endregion

        #region property

        /// <summary>
        /// あんまり作りたかなかったけどしゃあない。
        /// </summary>
        /// <param name="logger"></param>
        protected void ResetLogger(ILogger logger)
        {
            Logger = logger;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>時に呼び出されるイベント。
        /// <para>呼び出し時点では<see cref="IsDisposed"/>は偽のまま。</para>
        /// </summary>
        [field: NonSerialized]
        public event EventHandler Disposing;

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>されたか。
        /// </summary>
        [IgnoreDataMember, XmlIgnore]
        public bool IsDisposed { get; private set; }

        protected void ThrowIfDisposed([CallerMemberName] string _callerMemberName = "")
        {
            if(IsDisposed) {
                throw new ObjectDisposedException(_callerMemberName);
            }
        }


        /// <summary>
        /// <see cref="IDisposable.Dispose"/>の内部処理。
        /// <para>継承先クラスでは本メソッドを呼び出す必要がある。</para>
        /// </summary>
        /// <param name="disposing">CLRの管理下か。</param>
        protected virtual void Dispose(bool disposing)
        {
            if(IsDisposed) {
                return;
            }

            if(Disposing != null) {
                Disposing(this, EventArgs.Empty);
            }

            if(disposing) {
                if(Logger is IDisposable disposer) {
                    disposer.Dispose();
                }

                GC.SuppressFinalize(this);
            }

            IsDisposed = true;
        }

        /// <summary>
        /// 解放。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

    }

    public class RawModel : ModelBase
    {
        public RawModel(object rawObject)
        {
            RawObject = rawObject;
        }

        #region property

        public object RawObject { get; }

        #endregion
    }

    public class RawModel<T> : RawModel
    {
        public RawModel(T rawObject)
            : base(rawObject)
        {
            Raw = rawObject;
        }

        #region property

        public T Raw { get; }

        #endregion
    }
}
