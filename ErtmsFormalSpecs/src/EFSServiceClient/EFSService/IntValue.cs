using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFSServiceClient.EFSService
{
    /// <summary>
    ///     Manually written code to access EFSModel
    /// </summary>
    public partial class IntValue
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        public IntValue(int value)
        {
            Image = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        public IntValue(string value)
        {
            Image = value;
        }

        /// <summary>
        /// Provides the integer value for this
        /// </summary>
        public int Value
        {
            get
            {
                int retVal = 0;

                int.TryParse(Image, out retVal);

                return retVal;
            }
            set
            {
                Image = value.ToString(CultureInfo.InvariantCulture);                
            }
        }

        /// <summary>
        ///     Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public override string DisplayValue()
        {
            return Image;
        }
    }
}
