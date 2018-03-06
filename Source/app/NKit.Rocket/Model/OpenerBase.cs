using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Rocket.Model
{
    public abstract class OpenerBase: DisposerBase
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

    public abstract class ComOpenerBase: OpenerBase
    {
        public ComOpenerBase(string filePath)
            : base(filePath)
        { }

        #region property
        protected bool ExcelQuit { get; set; } = true;

        #endregion

    }
}
