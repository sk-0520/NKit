using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using Microsoft.Extensions.CommandLineUtils;

namespace ContentTypeTextNet.NKit.Rocket.Model
{
    public class RocketModel : CliApplicationModelBase
    {
        public RocketModel(string[] arguments)
            : base(arguments)
        { }

        #region property

        AssociationFileKind AssociationFileKind { get; set; }

        string FilePath { get; set; }

        #region spreadsheet

        string SpreadSheetSheetName { get; set; }
        int SpreadSheetRowIndex { get; set; }
        int SpreadSheetColumnIndex { get; set; }

        string SpreadSheetCellAddress { get; set; }

        #endregion

        #region document

        int DocumentLineNumber { get; set; }
        int DocumentCharacterPosition { get; set; }
        int DocumentLength { get; set; }
        int DocumentPageNumber { get; set; }

        #endregion

        #endregion

        #region function

        void OpenMicrosoftExcel()
        {
            var eo = new MicrosoftExcelOpener(FilePath, SpreadSheetSheetName, SpreadSheetRowIndex, SpreadSheetColumnIndex);
            eo.Open();
        }

        void OpenMicrosoftWord()
        {
            var wo = new MicrosoftWordOpener(FilePath, DocumentLineNumber, DocumentCharacterPosition, DocumentLength, DocumentPageNumber);
            wo.Open();
        }


        #endregion


        #region ApplicationModelBase

        protected override PreparaResult<int> PreparationCore(CancellationToken cancelToken)
        {
            var optionKind = CommandLineApplication.Option("--kind", $"{nameof(AssociationFileKind)}", CommandOptionType.SingleValue);
            var optionPath = CommandLineApplication.Option("--path", $"file path", CommandOptionType.SingleValue);

            var optionSpreadSheetName = CommandLineApplication.Option("--spreadsheet_name", $"sheet name", CommandOptionType.SingleValue);
            var optionSpreadSheetX = CommandLineApplication.Option("--spreadsheet_x", $"row index (0 base)", CommandOptionType.SingleValue);
            var optionSpreadSheetY = CommandLineApplication.Option("--spreadsheet_y", $"col index (0 base)", CommandOptionType.SingleValue);
            var optionSpreadSheetCell = CommandLineApplication.Option("--spreadsheet_cell", "cell address", CommandOptionType.SingleValue);

            var optionDocumentLineNumber = CommandLineApplication.Option("--document_line", "line number", CommandOptionType.SingleValue);
            var optionDocumentCharacterPosition = CommandLineApplication.Option("--document_position", "line number", CommandOptionType.SingleValue);
            var optionDocumentLength = CommandLineApplication.Option("--document_length", "line number", CommandOptionType.SingleValue);
            var optionDocumentPageNumber = CommandLineApplication.Option("--document_page", "page number", CommandOptionType.SingleValue);

            if(!BuildCommandLine()) {
                return GetDefaultPreparaValue(false);
            }

            AssociationFileKind = (AssociationFileKind)Enum.Parse(typeof(AssociationFileKind), optionKind.Value());
            FilePath = optionPath.Value();

            switch(AssociationFileKind) {
                case AssociationFileKind.MicrosoftOfficeExcel:
                    SpreadSheetSheetName = optionSpreadSheetName.Value();
                    SpreadSheetColumnIndex = int.Parse(optionSpreadSheetX.Value());
                    SpreadSheetRowIndex = int.Parse(optionSpreadSheetY.Value());
                    SpreadSheetCellAddress = optionSpreadSheetCell.Value();
                    break;

                case AssociationFileKind.MicrosoftOfficeWord:
                    DocumentLineNumber = int.Parse(optionDocumentLineNumber.Value());
                    DocumentCharacterPosition = int.Parse(optionDocumentCharacterPosition.Value());
                    DocumentLength = int.Parse(optionDocumentLength.Value());
                    DocumentPageNumber = int.Parse(optionDocumentPageNumber.Value());
                    break;

                default:
                    break;
            }

            return base.PreparationCore(cancelToken);
        }

        protected override int RunCore(CancellationToken cancelToken)
        {
            switch(AssociationFileKind) {
                case AssociationFileKind.MicrosoftOfficeExcel:
                    OpenMicrosoftExcel();
                    break;

                case AssociationFileKind.MicrosoftOfficeWord:
                    OpenMicrosoftWord();
                    break;

                default:
                    break;
            }

            return 0;
        }

        #endregion

    }
}
