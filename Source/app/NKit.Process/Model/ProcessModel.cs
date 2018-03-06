using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using Microsoft.Extensions.CommandLineUtils;

namespace ContentTypeTextNet.NKit.Process.Model
{
    public class ProcessModel : ApplicationModelBase
    {
        public ProcessModel(string[] arguments)
            : base(arguments)
        { }

        #region property

        AssociationFileKind AssociationFileKind { get; set; }

        string FilePath { get; set; }

        string SpreadSheetSheetName { get; set; }
        int SpreadSheetRowIndex { get; set; }
        int SpreadSheetColumnIndex { get; set; }

        #endregion

        #region function

        void OpenMicrosoftExcel()
        {
            var eo = new ExcelOpener(FilePath, SpreadSheetSheetName, SpreadSheetRowIndex, SpreadSheetColumnIndex);
            eo.Open();
        }

        #endregion


        #region ApplicationModelBase

        protected override Task<PreparaResult<int>> PreparationCoreAsync(CancellationToken cancelToken)
        {
            var optionKind = CommandLineApplication.Option("--kind", $"{nameof(AssociationFileKind)}", CommandOptionType.SingleValue);
            var optionPath = CommandLineApplication.Option("--path", $"file path", CommandOptionType.SingleValue);

            var optionSpreadSheetName = CommandLineApplication.Option("--ss_sheet", $"sheet name", CommandOptionType.SingleValue);
            var optionSpreadSheetX = CommandLineApplication.Option("--ss_x", $"row index (0 base)", CommandOptionType.SingleValue);
            var optionSpreadSheetY = CommandLineApplication.Option("--ss_y", $"col index (0 base)", CommandOptionType.SingleValue);

            if(!BuildCommandLine()) {
                return GetDefaultPreparaValueTask(false);
            }

            AssociationFileKind = (AssociationFileKind)Enum.Parse(typeof(AssociationFileKind), optionKind.Value());
            FilePath = optionPath.Value();

            if(AssociationFileKind == AssociationFileKind.MicrosoftOfficeExcel) {
                SpreadSheetSheetName = optionSpreadSheetName.Value();
                SpreadSheetColumnIndex = int.Parse(optionSpreadSheetX.Value());
                SpreadSheetRowIndex = int.Parse(optionSpreadSheetY.Value());
            }

            return base.PreparationCoreAsync(cancelToken);
        }

        protected override Task<int> RunCoreAsync(CancellationToken cancelToken)
        {
            switch(AssociationFileKind) {
                case AssociationFileKind.MicrosoftOfficeExcel:
                    OpenMicrosoftExcel();
                    break;

                default:
                    break;
            }
            return Task.FromResult(0);
        }

        #endregion

    }
}
