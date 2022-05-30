using Assist.Enums;

using System;
using System.Globalization;
using System.Windows.Data;

namespace Assist.Converters;

[ValueConversion(typeof(Enum), typeof(string))]
public class LanguageEnumConverter : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return "English";

        var val = (ELanguage)value!;
        return val.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

}
