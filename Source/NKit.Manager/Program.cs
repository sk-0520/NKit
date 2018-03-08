using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Manager.Model;

namespace ContentTypeTextNet.NKit.Manager
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var worker = new ManagerWorker();
            worker.Initialize();
            worker.LoadSetting();

            if(worker.CheckNeedAccept()) {
                using(var acceptForm = new View.AcceptForm()) {
                    acceptForm.SetWorker(worker);

                    Application.Run(acceptForm);

                    worker.Accepted = acceptForm.DialogResult == DialogResult.OK;
                }
                if(!worker.Accepted) {
                    return;
                }
            }

            var managerForm = new View.ManagerForm();
            managerForm.SetWorker(worker);

            Application.Run(managerForm);

            if(worker.NeedSave) {
                worker.SaveSetting();
            }
        }
    }
}
