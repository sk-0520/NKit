using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.View.Control.AvalonEditExtension;
using ContentTypeTextNet.NKit.Utility.Model;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace ContentTypeTextNet.NKit.Main.View.Control
{
    /// <summary>
    /// TextSearchMatchControl.xaml の相互作用ロジック
    /// </summary>
    public partial class TextSearchMatchControl : UserControl
    {
        public TextSearchMatchControl()
        {
            InitializeComponent();
        }

        #region InformationFontFamily

        public FontFamily InformationFontFamily
        {
            get { return (FontFamily)GetValue(InformationFontFamilyProperty) ?? FontFamily; }
            set { SetValue(InformationFontFamilyProperty, value); }
        }

        public static readonly DependencyProperty InformationFontFamilyProperty = DependencyProperty.Register(
            nameof(InformationFontFamily),
            typeof(FontFamily),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(new FontFamily("MS Gothic"), InformationFontFamilyPropertyChanged)
        );

        private static void InformationFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).InformationFontFamily = (FontFamily)e.NewValue;
            ((TextSearchMatchControl)d).BuildMatchItems();
        }

        #endregion

        #region InformationForeground

        public Brush InformationForeground
        {
            get { return (Brush)GetValue(InformationForegroundProperty) ?? Foreground; }
            set { SetValue(InformationForegroundProperty, value); }
        }

        public static readonly DependencyProperty InformationForegroundProperty = DependencyProperty.Register(
            nameof(InformationForeground),
            typeof(Brush),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(default(Brush), InformationForegroundPropertyChanged)
        );

        private static void InformationForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).InformationForeground = (Brush)e.NewValue;
            ((TextSearchMatchControl)d).BuildMatchItems();
        }

        #endregion

        #region InformationBackground

        public Brush InformationBackground
        {
            get { return (Brush)GetValue(InformationBackgroundProperty) ?? Background; }
            set { SetValue(InformationBackgroundProperty, value); }
        }

        public static readonly DependencyProperty InformationBackgroundProperty = DependencyProperty.Register(
            nameof(InformationBackground),
            typeof(Brush),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(Brushes.LightGray, InformationBackgroundPropertyChanged)
        );

        private static void InformationBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).InformationBackground = (Brush)e.NewValue;
            ((TextSearchMatchControl)d).BuildMatchItems();
        }

        #endregion

        #region MatchForeground

        public Brush MatchForeground
        {
            get { return (Brush)GetValue(MatchForegroundProperty) ?? Foreground; }
            set { SetValue(MatchForegroundProperty, value); }
        }

        public static readonly DependencyProperty MatchForegroundProperty = DependencyProperty.Register(
            nameof(MatchForeground),
            typeof(Brush),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(default(Brush), MatchForegroundPropertyChanged)
        );

        private static void MatchForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).MatchForeground = (Brush)e.NewValue;
        }

        #endregion

        #region MatchBackground

        public Brush MatchBackground
        {
            get { return (Brush)GetValue(MatchBackgroundProperty) ?? Background; }
            set { SetValue(MatchBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MatchBackgroundProperty = DependencyProperty.Register(
            nameof(MatchBackground),
            typeof(Brush),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(Brushes.Pink, MatchBackgroundPropertyChanged)
        );

        private static void MatchBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).MatchBackground = (Brush)e.NewValue;
        }

        #endregion

        #region MatchFontWeight

        public FontWeight MatchFontWeight
        {
            get { return (FontWeight)GetValue(MatchFontWeightProperty); }
            set { SetValue(MatchFontWeightProperty, value); }
        }

        public static readonly DependencyProperty MatchFontWeightProperty = DependencyProperty.Register(
            nameof(MatchFontWeight),
            typeof(FontWeight),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(FontWeights.Bold, MatchFontWeightPropertyChanged)
        );

        private static void MatchFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).MatchFontWeight = (FontWeight)e.NewValue;
        }

        #endregion

        #region ItemsSource

        public IEnumerable<TextSearchMatch> ItemsSource
        {
            get { return (IEnumerable<TextSearchMatch>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable<TextSearchMatch>),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(Enumerable.Empty<TextSearchMatch>(), PropertyChangedCallback)
        );

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).ItemsSource = (IEnumerable<TextSearchMatch>)e.NewValue;
            ((TextSearchMatchControl)d).BuildMatchItems();
        }

        #endregion

        #region HiddenTopLineOnly

        public bool HiddenTopLineOnly
        {
            get { return (bool)GetValue(HiddenTopLineOnlyProperty); }
            set { SetValue(HiddenTopLineOnlyProperty, value); }
        }

        public static readonly DependencyProperty HiddenTopLineOnlyProperty = DependencyProperty.Register(
            nameof(HiddenTopLineOnly),
            typeof(bool),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(true, HiddenTopLineOnlyPropertyChanged)
        );

        private static void HiddenTopLineOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).HiddenTopLineOnly = (bool)e.NewValue;
        }

        #endregion

        #region GroupingOneLineCharacters

        public bool GroupingOneLineCharacters
        {
            get { return (bool)GetValue(GroupingOneLineCharactersProperty); }
            set { SetValue(GroupingOneLineCharactersProperty, value); }
        }

        public static readonly DependencyProperty GroupingOneLineCharactersProperty = DependencyProperty.Register(
            nameof(GroupingOneLineCharacters),
            typeof(bool),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(false, GroupingOneLineCharactersPropertyChanged)
        );

        private static void GroupingOneLineCharactersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).GroupingOneLineCharacters = (bool)e.NewValue;
        }

        #endregion

        #region ShowSingleLine

        public bool ShowSingleLine
        {
            get { return (bool)GetValue(ShowSingleLineProperty); }
            set { SetValue(ShowSingleLineProperty, value); }
        }

        public static readonly DependencyProperty ShowSingleLineProperty = DependencyProperty.Register(
            nameof(ShowSingleLine),
            typeof(bool),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(false, ShowSingleLinePropertyChanged)
        );

        private static void ShowSingleLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).ShowSingleLine = (bool)e.NewValue;
        }

        #endregion

        #region ShowSingleUnmatchText

        public string ShowSingleUnmatchText
        {
            get { return (string)GetValue(ShowSingleUnmatchTextProperty); }
            set { SetValue(ShowSingleUnmatchTextProperty, value); }
        }

        public static readonly DependencyProperty ShowSingleUnmatchTextProperty = DependencyProperty.Register(
            nameof(ShowSingleUnmatchText),
            typeof(string),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(string.Empty, ShowSingleUnmatchTextPropertyChanged)
        );

        private static void ShowSingleUnmatchTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).ShowSingleUnmatchText = (string)e.NewValue;
        }

        #endregion

        #region IsSelectable

        public bool IsSelectable
        {
            get { return (bool)GetValue(IsSelectableProperty); }
            set { SetValue(IsSelectableProperty, value); }
        }

        public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register(
            nameof(IsSelectable),
            typeof(bool),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(true, IsSelectablePropertyChanged)
        );

        private static void IsSelectablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).IsSelectable = (bool)e.NewValue;
        }

        #endregion

        #region UserSelectedCommand

        public ICommand UserSelectedCommand
        {
            get { return (ICommand)GetValue(UserSelectedCommandProperty); }
            set { SetValue(UserSelectedCommandProperty, value); }
        }

        public static readonly DependencyProperty UserSelectedCommandProperty = DependencyProperty.Register(
            nameof(UserSelectedCommand),
            typeof(ICommand),
            typeof(TextSearchMatchControl),
            new PropertyMetadata(default(ICommand), UserSelectedCommandPropertyChanged)
        );

        private static void UserSelectedCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextSearchMatchControl)d).UserSelectedCommand = (ICommand)e.NewValue;
        }

        #endregion

        #region function

        string FormatNumer(int value, int length)
        {
            var f = "{0, " + length.ToString() + "}";
            return string.Format(f, value);
        }

        IEnumerable<Inline> BuildSingleInlines(IReadOnlyList<TextSearchMatch> matches)
        {
            var line = matches.First().LineText;

            var nextStartIndex = 0;

            foreach(var match in matches) {
                var headText = line.Substring(nextStartIndex, match.CharacterPostion - nextStartIndex);
                var bodyText = line.Substring(match.CharacterPostion, match.Length);
                nextStartIndex = match.CharacterPostion + match.Length;

                var headElement = new Run(headText);
                var bodyElement = new Run(bodyText) {
                    Foreground = MatchForeground,
                    Background = MatchBackground,
                    FontWeight = MatchFontWeight,
                };

                yield return headElement;
                yield return bodyElement;
            }

            var tailText = line.Substring(nextStartIndex);
            var tailElement = new Run(tailText);

            yield return tailElement;
        }

        void BuildMatchItemsSingleLine(IReadOnlyList<TextSearchMatch> matches)
        {
            var p = new Paragraph();
            var inlines = new List<Inline>();
            if(matches.Any()) {
                // いや～、これはどうにもならんだろ
                p.Tag = matches.First();
                inlines.AddRange(BuildSingleInlines(matches));
            } else {
                inlines.Add(new Run(ShowSingleUnmatchText));
            }

            // 覚書: 呼び出し時にクリア済み
            if(IsSelectable) {
                throw new NotImplementedException();
            } else {
                this.viewSingleLineMatchItems.Inlines.AddRange(inlines);
                this.viewSingleLineMatchItems.Visibility = Visibility.Visible;
            }
        }

        IEnumerable<Block> BuildmatchItemsGroupingMultiLine(IReadOnlyList<TextSearchMatch> matches, bool hasHeader, bool hasFooter, bool showLine)
        {
            throw new NotImplementedException("だりぃ");
        }

        void BuildmatchItemsAllMultiLine(IReadOnlyList<TextSearchMatch> matches, bool hasHeader, bool hasFooter, bool showLine)
        {
            this.viewMatchItems.Text = string.Join(Environment.NewLine, matches.Select(m => m.LineText));
            var highlighter = new TextSearchMatchallLinesHighlighter(matches, MatchForeground, MatchBackground, MatchFontWeight);

            this.viewMatchItems.TextArea.LeftMargins.Clear();
            this.viewMatchItems.TextArea.LeftMargins.Add(new InformationMargin(matches, InformationForeground, InformationBackground, InformationFontFamily));
            this.viewMatchItems.TextArea.TextView.LineTransformers.Add(highlighter);
        }

        void BuildMatchItemsMultiLine(IReadOnlyList<TextSearchMatch> matches)
        {
            var hasHeader = matches.Any(m => m.Header != null);
            var hasFooter = matches.Any(m => m.Footer != null);

            var showLine = !(HiddenTopLineOnly && matches.All(m => m.LineNumber == 1));

            this.viewMatchItems.TextArea.TextView.LineTransformers.Clear();

            if(GroupingOneLineCharacters) {
                BuildmatchItemsGroupingMultiLine(matches, hasHeader, hasFooter, showLine);
            } else {
                BuildmatchItemsAllMultiLine(matches, hasHeader, hasFooter, showLine);
            }

            this.viewMatchItems.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
            this.viewMatchItems.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            this.viewMatchItems.TextArea.PreviewKeyDown -= viewMatchItems_KeyDown;
            this.viewMatchItems.TextArea.PreviewKeyDown += viewMatchItems_KeyDown;
            this.viewMatchItems.TextArea.MouseWheel -= TextArea_MouseWheel;
            this.viewMatchItems.TextArea.MouseWheel += TextArea_MouseWheel;
            this.viewMatchItems.Visibility = Visibility.Visible;
        }

        void BuildMatchItems()
        {
            this.viewSingleLineMatchItems.Inlines.Clear();

            this.viewMatchItems.Visibility = Visibility.Collapsed;
            this.viewSingleLineMatchItems.Visibility = Visibility.Collapsed;

            if(ItemsSource == null) {
                return;
            }

            var matches = ItemsSource.Cast<TextSearchMatch>().ToList();

            // 一行表示の場合、データがそもそも全部同じ行じゃないと無理
            if(ShowSingleLine) {
                if(matches.Any() && 1 < matches.GroupBy(m => m.LineNumber).Count()) {
                    throw new InvalidOperationException(nameof(ShowSingleLine));
                }
                BuildMatchItemsSingleLine(matches);
            } else if(matches.Any()) {
                BuildMatchItemsMultiLine(matches);
            }
        }

        bool ExecuteUserSelectedCommand(TextSearchMatch match)
        {
            if(!IsSelectable) {
                return false;
            }

            if(UserSelectedCommand == null) {
                return false;
            }

            if(!UserSelectedCommand.CanExecute(match)) {
                return false;
            }

            UserSelectedCommand.Execute(match);

            return true;
        }

        TextSearchMatch GetCurrentMatchItem()
        {
            var lineNumber = this.viewMatchItems.TextArea.Caret.Line;
            if(lineNumber - 1 < ItemsSource.Count()) {
                return ItemsSource.ElementAt(lineNumber - 1);
            }

            return TextSearchMatch.Unmatch;
        }

        #endregion

        #region UserControl


        public override void OnApplyTemplate()
        {
            // 構築タイミングわっかんねぇ
            BuildMatchItems();

            base.OnApplyTemplate();
        }

        #endregion

        private void viewMatchItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var match = GetCurrentMatchItem();
            if(match.IsMatch) {
                if(ExecuteUserSelectedCommand(match)) {
                    e.Handled = true;
                }
            }
        }

        private void viewMatchItems_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) {
                var match = GetCurrentMatchItem();
                if(match.IsMatch) {
                    if(ExecuteUserSelectedCommand(match)) {
                        e.Handled = true;
                    }
                }
            }
        }
        private void TextArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(!e.Handled) {
                e.Handled = true;

                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;

                var scrollViewer = UIUtility.GetVisualClosest<ScrollViewer>(this);
                if(scrollViewer is UIElement element) {
                    element.RaiseEvent(eventArg);
                }

            }
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            var scrollViewer = UIUtility.GetVisualClosest<ScrollViewer>(this) as ScrollViewer;
            if(scrollViewer != null) {
                var y = this.viewMatchItems.TextArea.TextView.DefaultLineHeight * this.viewMatchItems.TextArea.Caret.Line;
                Debug.WriteLine(y);
                Debug.WriteLine(scrollViewer.ContentVerticalOffset);
                Debug.WriteLine(scrollViewer.VerticalOffset);

                // キャレットが表示外に行ったら表示してあげる
                if(y < scrollViewer.VerticalOffset) {
                    // 上へのスクロールは素直な動作
                    scrollViewer.ScrollToVerticalOffset(y);
                } else if(scrollViewer.ActualHeight + scrollViewer.VerticalOffset < y + this.viewMatchItems.TextArea.TextView.DefaultLineHeight * 2) {
                    // 下へのスクロールは補正しないと見た目が悪い(ガックガックなる)
                    // * 2 は体感
                    scrollViewer.ScrollToVerticalOffset(y - scrollViewer.ActualHeight + this.viewMatchItems.TextArea.TextView.DefaultLineHeight * 2);
                }
            }

        }


    }
}
