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
            var logger = worker.CreateLogger("ENTRY");
            worker.LoadSetting();

            if(worker.CheckNeedAccept()) {
                logger.Debug("show accept window");
                using(var acceptForm = new View.AcceptForm()) {
                    acceptForm.SetWorker(worker);

                    Application.Run(acceptForm);

                    var accepted = acceptForm.DialogResult == DialogResult.OK;
                    worker.Accepted = accepted;
                    if(!worker.Accepted) {
                        logger.Debug("not accept");
                        return;
                    }
                }
            }

            var managerForm = new View.ManagerForm();
            managerForm.SetWorker(worker);

            logger.Debug("show manager window");
            Application.Run(managerForm);

            logger.Debug($"save?: {worker.NeedSave}");
            if(worker.NeedSave) {
                worker.SaveSetting();
            }

            worker.Dispose();
        }
    }
}
