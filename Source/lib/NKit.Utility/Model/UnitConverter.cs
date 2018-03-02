using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public class UnitConverter
    {
        #region function

        public int GetNumberWidth(int value)
        {
            if(value == 0) {
                value += 1;
            }

            return (int)(Math.Log10(value) + 1);
        }

        #endregion
    }
}
