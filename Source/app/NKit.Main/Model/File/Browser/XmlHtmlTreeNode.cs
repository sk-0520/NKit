using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace ContentTypeTextNet.NKit.Main.Model.File.Browser
{
    public abstract class XmlHtmlTreeNodeBase
    {
        #region property

        public bool IsExpanded { get; } = true;

        public abstract IEnumerable<XmlHtmlTreeNodeBase> ChildNodes { get; }

        public abstract string Name { get; }

        #endregion
    }

    public sealed class HtmlTreeNode : XmlHtmlTreeNodeBase
    {
        public HtmlTreeNode(HtmlNode node)
        {
            Node = node;
        }

        #region property

        HtmlNode Node { get; }

        #endregion

        #region NodeBase

        public override IEnumerable<XmlHtmlTreeNodeBase> ChildNodes => Node.ChildNodes.Cast<HtmlNode>().Select(n => new HtmlTreeNode(n));

        public override string Name => Node.OriginalName;

        #endregion
    }

    public sealed class XmlTreeNode : XmlHtmlTreeNodeBase
    {
        public XmlTreeNode(XmlNode node)
        {
            Node = node;
        }

        #region property

        XmlNode Node { get; }

        #endregion

        #region NodeBase

        public override IEnumerable<XmlHtmlTreeNodeBase> ChildNodes => Node.ChildNodes.Cast<XmlNode>().Select(n => new XmlTreeNode(n));

        public override string Name => Node.Name;

        #endregion
    }
}
