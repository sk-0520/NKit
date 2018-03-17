using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Manager.Model.Workspace
{
    public interface IReadOnlyActiveWorkspace
    {
        #region property

        string BaseId { get; }

        /// <summary>
        /// あぷりけーしょんあいでぃー。
        /// </summary>
        string ApplicationId { get; }

        /// <summary>
        /// サービスURI。
        /// </summary>
        Uri ServiceUri { get; }

        /// <summary>
        /// ワークスペース終了イベント名。
        /// </summary>
        string GroupSuicideEventName { get; }


        #endregion
    }

    /// <summary>
    ///
    /// </summary>
    public class ActiveWorkspace : IReadOnlyActiveWorkspace
    {
        #region property

        public string LogFilePath { get; set; }
        public TextWriter LogWriter { get; set; }

        #endregion

        #region IReadOnlyActiveWorkspace

        public string BaseId { get; set; }

        public string ApplicationId { get; set; }
        /// <summary>
        /// サービスURI。
        /// </summary>
        public Uri ServiceUri { get; set; }

        /// <summary>
        /// 終了イベント名。
        /// </summary>
        public string GroupSuicideEventName { get; set; }

        #endregion
    }
}
