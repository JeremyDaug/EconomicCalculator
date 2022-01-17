using EconomicCalculator.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EditorInterface.Converters
{
    class EnumListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(List<ParameterType>))
            {
                var values = (List<ParameterType>)value;
                var result = "";

                foreach (var item in values)
                {
                    result += item.ToString() + ";";
                }

                return result.Remove(result.Length - 1);
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
