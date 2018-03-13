using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public abstract class CameraBase: ModelBase
    {
        #region property

        #endregion

        #region function

        /// <summary>
        /// 設定に基づいてキャプチャ取得。
        /// </summary>
        /// <returns></returns>
        protected abstract Image TaskShotCore();

        /// <summary>
        /// 設定に基づいてキャプチャ取得。
        /// </summary>
        /// <returns></returns>
        public Image TaskShot()
        {
            return TaskShotCore();
        }

        #endregion
    }
}
