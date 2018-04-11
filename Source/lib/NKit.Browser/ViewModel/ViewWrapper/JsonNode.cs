using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.ViewModel;
using Newtonsoft.Json.Linq;

namespace ContentTypeTextNet.NKit.Browser.ViewModel.ViewWrapper
{
    public class JsonNode: ViewModelBase, INodeHeader
    {
        #region variable

        IList<JsonNode> _childNodes = null;

        bool _isSelected;

        #endregion

        public JsonNode(JToken token)
        {
            Token = token;
            TokenType = Token.GetType();
        }

        #region property
        public bool IsExpanded { get; } = true;

        public JToken Token { get; }
        Type TokenType { get; }


        public IEnumerable<JsonNode> ChildNodes
        {
            get
            {
                if(this._childNodes == null) {
                    if(IsValue) {
                        this._childNodes = Enumerable.Empty<JsonNode>().ToList();
                    }

                    if(IsProperty) {
                        var property = (JProperty)Token;
                        this._childNodes = property.Children().Select(j => new JsonNode(j)).ToList();
                    } else if(IsArray) {
                        var property = (JArray)Token;
                        this._childNodes = property.Children().Select(j => new JsonNode(j)).ToList();
                    } else if(IsObject) {
                        var property = (JObject)Token;
                        this._childNodes = property.Children().Select(j => new JsonNode(j)).ToList();
                    }

                    if(this._childNodes == null) {
                        throw new NotSupportedException(Token.Type.ToString());
                    }
                }

                return this._childNodes;
            }
        }

        public int ChildCount => ChildNodes.Count();

        public bool IsProperty => Token.Type == JTokenType.Property;
        public bool IsArray => Token.Type == JTokenType.Array;
        public bool IsObject => Token.Type == JTokenType.Object;

        public bool IsValue => !(IsProperty || IsArray || IsObject);

        public bool ValueIsNull => IsValue && ((JValue)Token).Value == null;

        public string Name
        {
            get
            {
                if(IsProperty) {
                    var property = (JProperty)Token;
                    return property.Name;
                }

                if(IsArray || IsObject) {
                    return $"[{ChildCount}]";
                }

                Debug.Assert(IsValue);

                if(ValueIsNull) {
                    return "<null>";
                }

                var jvalue = (JValue)Token;
                var value = jvalue.Value;
                return value.ToString();
            }
        }

        #endregion

        #region INodeHeader

        public string DisplayHeader => Name;

        public bool IsSelected
        {
            get { return this._isSelected; }
            set { SetProperty(ref this._isSelected, value); }
        }

        #endregion
    }
}
