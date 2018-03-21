using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Utility.View.Attach
{
    public class WindowTitleBehavior : Behavior<Window>
    {
        #region TitleProperty

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(WindowTitleBehavior),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnTitleChanged))
        );

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as WindowTitleBehavior;
            if(control != null) {
                control.Title = e.NewValue as string;
            }
        }

        public string Title
        {
            get { return GetValue(TitleProperty) as string; }
            set
            {
                SetValue(TitleProperty, value);
                ChangeTitle();
            }
        }

        #endregion

        #region function

        void ChangeTitle()
        {
            if(AssociatedObject != null) {
                AssociatedObject.Title = CommonUtility.ReplaceWindowTitle(Title);
            }
        }

        #endregion

        // 要素にアタッチされたときの処理。大体イベントハンドラの登録処理をここでやる
        protected override void OnAttached()
        {
            base.OnAttached();
            ChangeTitle();
        }
    }
}
