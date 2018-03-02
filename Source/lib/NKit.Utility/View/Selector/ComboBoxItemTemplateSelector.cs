using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Utility.View.Selector
{
    public class ComboBoxItemTemplateSelector : DataTemplateSelector
    {
        #region property

        public DataTemplate DropDownTemplate { get; set; }
        public DataTemplate SelectedTemplate { get; set; }

        #endregion

        #region DataTemplateSelector

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var comboBoxItem = UIUtility.GetVisualClosest<ComboBoxItem>(container);
            if(comboBoxItem != null) {
                return DropDownTemplate;
            }

            return SelectedTemplate;
        }

        #endregion
    }
}
