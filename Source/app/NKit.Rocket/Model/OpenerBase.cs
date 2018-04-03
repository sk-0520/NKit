using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.Model.Unmanaged;

namespace ContentTypeTextNet.NKit.Rocket.Model
{
    public abstract class OpenerBase: ModelBase
    {
        public OpenerBase(string filePath)
        {
            FilePath = filePath;
        }

        #region property

        public string FilePath { get; }

        #endregion

        #region function

        public abstract bool Open();

        #endregion
    }

    public abstract class ComApplicationOpenerBase: OpenerBase
    {
        public ComApplicationOpenerBase(string filePath)
            : base(filePath)
        { }

        #region property
        protected bool CanApplicationQuit { get; set; } = true;
        protected Action ApplicationQuitAction { get; set; }

        #endregion

        #region function

        protected void QuitAppication()
        {
            // https://blogs.msdn.microsoft.com/office_client_development_support_blog/2012/02/09/office-5/

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            if(CanApplicationQuit) {
                ApplicationQuitAction();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        #endregion

    }

}
