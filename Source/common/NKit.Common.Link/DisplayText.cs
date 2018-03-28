using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Common
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public abstract class DisplayTextAttributeBase : System.Attribute
    {
        protected DisplayTextAttributeBase(string value)
        {
            Value = value;
        }

        #region property

        public string Value { get; }

        public abstract string Text { get; }

        #endregion
    }

    public class DisplayTextAttribute : DisplayTextAttributeBase
    {
        public DisplayTextAttribute(string text)
            : base(text)
        { }

        #region DisplayAttributeBase

        public override string Text { get { return Value; } }

        #endregion
    }

    public class EnumResourceDisplayAttribute : DisplayTextAttributeBase
    {
        static EnumResourceDisplayAttribute()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var baseName = fileVersionInfo.ProductName + ".Properties.Resources";
            ResourceManager = new global::System.Resources.ResourceManager(baseName, assembly);
        }

        static ResourceManager ResourceManager { get; }

        public EnumResourceDisplayAttribute(string name)
            : base(name)
        { }

        #region DisplayAttributeBase

        public override string Text
        {
            get
            {
                return ResourceManager.GetString(Value);
            }
        }

        #endregion

    }

    /// <summary>
    /// <see cref="DisplayTextAttributeBase"/>に対する処理。
    /// </summary>
    public static class DisplayTextUtility
    {
        public static string GetDisplayText(object value)
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString()).FirstOrDefault();
            if(memberInfo != null) {
                var attrs = memberInfo.GetCustomAttributes(typeof(DisplayTextAttributeBase), true);
                if(attrs != null && attrs.Length > 0) {
                    var display = ((DisplayTextAttributeBase)attrs[0]);
                    return display.Text;
                }
            }

            return value.ToString();
        }
    }

}
