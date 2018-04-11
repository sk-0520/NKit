using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.Browser.ViewModel.ViewWrapper
{
    interface INodeHeader
    {
        #region property

        string DisplayHeader { get; }

        bool IsSelected { get; set; }

        #endregion
    }

    public class HeaderNode<TViewModel>
        where TViewModel: ViewModelBase
    {
        public HeaderNode(TViewModel viewModel, Point position)
        {
            Data = viewModel;
            Position = position;
        }

        #region property

        public TViewModel Data { get; }

        public Point Position { get; }

        #endregion
    }
}
