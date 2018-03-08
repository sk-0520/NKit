using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public class DisplayItemBase
    {
        public DisplayItemBase(object rawValue)
        {
            RawValue = rawValue;
        }

        #region property

        public Object RawValue { get; }

        public virtual string DisplayText => RawValue.ToString();

        #endregion

        #region object

        public override string ToString()
        {
            return DisplayText;
        }

        #endregion
    }

    public class DisplayItem<T> : DisplayItemBase
    {
        public DisplayItem(T value)
            : base(value)
        {
            Value = value;
        }

        #region property

        public T Value { get; }

        #endregion
    }

    public class CustomDisplayItem<T> : DisplayItem<T>
    {
        public CustomDisplayItem(T value)
            : base(value)
        { }

        #region property

        public Func<T, string> CustomDisplayText { get; set; }

        #endregion

        #region DisplayItem

        public override string DisplayText => CustomDisplayText(Value);

        #endregion
    }
}
