using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.AvalonEdit.Rendering;
using System.Globalization;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.View.Control.AvalonEditExtension
{
    public class InformationMargin : LineNumberMargin
    {

        public InformationMargin(IReadOnlyList<TextSearchMatch> matches, Brush informationForeground, Brush informationBackground, FontFamily informationFontFamily)
        {
            Matches = matches;
            InformationForeground = informationForeground;
            InformationBackground = informationBackground;
            InformationFontFamily = informationFontFamily;

            HasHeader = Matches.Any(m => m.Header != null);
            HasFooter = Matches.Any(m => m.Footer != null);

            SetValue(System.Windows.Controls.Control.ForegroundProperty, InformationForeground);
            SetValue(System.Windows.Controls.Control.BackgroundProperty, InformationBackground);
            SetValue(TextBlock.FontFamilyProperty, InformationFontFamily);
        }

        #region property

        IReadOnlyList<TextSearchMatch> Matches { get; }

        Brush InformationForeground { get; }
        Brush InformationBackground { get; }
        FontFamily InformationFontFamily { get; }

        bool HasHeader { get; }
        bool HasFooter { get; }

        #endregion

        #region LineNumberMargin

        protected override Size MeasureOverride(Size availableSize)
        {

            base.typeface = new Typeface(
                InformationFontFamily,
                (FontStyle)GetValue(TextBlock.FontStyleProperty),
                (FontWeight)GetValue(TextBlock.FontWeightProperty),
                (FontStretch)GetValue(TextBlock.FontStretchProperty)
            );
            base.emSize = (double)GetValue(TextBlock.FontSizeProperty);

            // どうせ全部出すし
            var targetItems = Matches
                .Select(m => new { Matche = m, HeaderLength = m.Header != null ? new StringInfo(m.Header.ToString()).LengthInTextElements: 0, FooterLength = m.Footer != null ? new StringInfo(m.Footer.ToString()).LengthInTextElements : 0 })
                .ToList()
            ;

            var uc = new UnitConverter();

            var maxLineNumberWidth = targetItems.Max(i => uc.GetNumberWidth(i.Matche.DisplayLineNumber));
            var maxCharacterPostionWidth = "()".Length + targetItems.Max(i => uc.GetNumberWidth(i.Matche.DisplayCharacterPostion));
            var maxHeaderWidth = targetItems.Max(i => i.HeaderLength);
            var maxFooterWidth = targetItems.Max(i => i.FooterLength);
            if(HasHeader) {
                maxHeaderWidth += "  ".Length;
            }
            if(HasFooter) {
                maxFooterWidth += "  ".Length;
            }

            var text = new FormattedText(
                new string('9', maxHeaderWidth + maxLineNumberWidth + maxCharacterPostionWidth + maxFooterWidth),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                base.typeface,
                base.emSize,
                InformationForeground
            );
            return new Size(text.Width, 0);
        }

        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var textView = TextView;
            if(textView != null && textView.VisualLinesValid) {
                var renderSize = RenderSize;
                drawingContext.DrawRectangle(InformationBackground, null,new Rect(0, 0, renderSize.Width, renderSize.Height));

                foreach(VisualLine line in textView.VisualLines) {
                    int lineNumber = line.FirstDocumentLine.LineNumber;
                    var match = Matches[lineNumber - 1];
                    // キッツいなぁ
                    var baseText = $"{match.DisplayLineNumber}({match.DisplayCharacterPostion})";
                    if(match.Header != null) {
                        baseText = $"{match.Header} {baseText}";
                    }
                    if(match.Footer != null) {
                        baseText = $"{baseText} {match.Footer}";
                    }

                    var text = new FormattedText(
                        baseText,
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        base.typeface,
                        base.emSize,
                        InformationForeground
                    );
                    var y = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextTop);
                    drawingContext.DrawText(text, new Point(0, y - textView.VerticalOffset));
                }
            }
        }

        #endregion
    }
}
