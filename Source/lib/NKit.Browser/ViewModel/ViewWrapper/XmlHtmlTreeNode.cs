using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.ViewModel;
using HtmlAgilityPack;

namespace ContentTypeTextNet.NKit.Browser.ViewModel.ViewWrapper
{
    public abstract class XmlHtmlTreeNodeBase: ViewModelBase, INodeHeader
    {
        #region define

        protected static IEnumerable<IGrouping<string, string>> NoneAttributes { get; } = Enumerable.Empty<IGrouping<string, string>>();

        #endregion

        #region variable

        bool _isSelected = false;
        bool _isExpanded = true;

        #endregion

        #region property

        public abstract IEnumerable<XmlHtmlTreeNodeBase> ChildNodesCore { get; }
        public IEnumerable<XmlHtmlTreeNodeBase> ChildNodes => ChildNodesCore.Where(n => n.Showable);

        public abstract string Name { get; }

        public abstract IEnumerable<IGrouping<string, string>> Attributes { get; }

        public bool HasAttribute => Attributes.Any();

        public abstract bool HasText { get; }
        public abstract string Text { get; }

        public bool IsNotEmptyText => !string.IsNullOrWhiteSpace(Text);

        public bool Showable => (HasText && IsNotEmptyText) || !HasText;

        #endregion

        #region function

        protected static string ConvertDisplayText(string text)
        {
            if(text == null) {
                return string.Empty;
            }

            var contents = TextUtility.ReadLines(text)
                .Select(s => s.Trim())
                .SkipWhile(s => string.IsNullOrEmpty(s))
                .TakeWhile(s => !string.IsNullOrEmpty(s))
            ;

            return string.Join(Environment.NewLine, contents);
        }

        #endregion

        #region INodeHeader

        public string DisplayHeader => Name;

        public bool IsSelected
        {
            get { return this._isSelected; }
            set { SetProperty(ref this._isSelected, value); }
        }

        public bool IsExpanded
        {
            get { return this._isExpanded; }
            set { SetProperty(ref this._isExpanded, value); }
        }

        public bool IsHeader => !HasText;

        #endregion
    }

    public sealed class HtmlTreeNode : XmlHtmlTreeNodeBase
    {
        #region variable

        IList<HtmlTreeNode> _children;

        #endregion

        public HtmlTreeNode(HtmlNode node)
        {
            Node = node;
        }

        #region property

        HtmlNode Node { get; }

        #endregion

        #region NodeBase

        public override IEnumerable<XmlHtmlTreeNodeBase> ChildNodesCore {
            get {
                if(this._children == null) {
                    this._children = Node.ChildNodes
                        .Cast<HtmlNode>()
                        .Select(n => new HtmlTreeNode(n))
                        .ToList()
                    ;
                }
                return this._children;
            }
        }

        public override string Name => Node.OriginalName;

        public override IEnumerable<IGrouping<string, string>> Attributes => Node.Attributes.GroupBy(a => a.OriginalName, a => a.Value);

        public override bool HasText => (Node.NodeType == HtmlNodeType.Document || Node.NodeType == HtmlNodeType.Comment || Node.NodeType == HtmlNodeType.Text);

        public override string Text => ConvertDisplayText(Node.InnerText);

        #endregion
    }

    public sealed class XmlTreeNode : XmlHtmlTreeNodeBase
    {
        #region variable

        IList<XmlTreeNode> _children;

        #endregion

        public XmlTreeNode(XmlNode node)
        {
            Node = node;
        }

        #region property

        XmlNode Node { get; }

        #endregion

        #region NodeBase

        public override IEnumerable<XmlHtmlTreeNodeBase> ChildNodesCore
        {
            get
            {
                if(this._children == null) {
                    this._children = Node.ChildNodes
                        .Cast<XmlNode>()
                        .Select(n => new XmlTreeNode(n))
                        .ToList()
                    ;
                }
                return this._children;
            }
        }

        public override string Name => Node.Name;

        public override IEnumerable<IGrouping<string, string>> Attributes => Node.Attributes?.Cast<XmlAttribute>().GroupBy(a => a.Name, a => a.Value) ?? NoneAttributes;

        public override bool HasText => (Node.NodeType == XmlNodeType.XmlDeclaration || Node.NodeType == XmlNodeType.Comment || Node.NodeType == XmlNodeType.Text);

        public override string Text => ConvertDisplayText(Node.InnerText);

        #endregion
    }

    public sealed class ExceptionNode : XmlHtmlTreeNodeBase
    {
        public ExceptionNode(Exception exception)
        {
            Exception = exception;
        }

        #region property

        Exception Exception { get; }

        #endregion

        #region XmlHtmlTreeNodeBase

        public override IEnumerable<XmlHtmlTreeNodeBase> ChildNodesCore => new[] { new ExceptionNode(Exception.InnerException) };

        public override string Name => Exception.GetType().Name;

        public override IEnumerable<IGrouping<string, string>> Attributes => NoneAttributes;

        public override bool HasText => Exception != null;

        public override string Text => Exception.ToString();

        #endregion
    }
}
