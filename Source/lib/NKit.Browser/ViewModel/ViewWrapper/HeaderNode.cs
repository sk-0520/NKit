using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.Browser.ViewModel.ViewWrapper
{
    public interface INodeHeader
    {
        #region property

        string DisplayHeader { get; }

        bool IsSelected { get; set; }

        bool IsHeader { get; }

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

    public static class HeaderNodeUtility
    {
        #region function

        public static IEnumerable<HeaderNode<TViewModel>> GetHeaderNodes<TViewModel>(TreeView treeView)
            where TViewModel: ViewModelBase, INodeHeader
        {
            var items = UIUtility.FindChildren<TreeViewItem>(treeView)
                .Where(t => t.IsVisible)
                .Select(t => new { View = t, Data = (TViewModel)t.DataContext })
                .Where(i => i.Data.IsHeader)
                .Select(i => new { i.View, i.Data, Position = i.View.TransformToAncestor(treeView).Transform(new Point(0, 0)) })
                .Where(i => 0 < i.Position.Y && i.Position.Y < treeView.ActualHeight)
                .Select(i => new HeaderNode<TViewModel>(i.Data, i.Position))
            ;

            return items;
        }

        #endregion
    }
}
