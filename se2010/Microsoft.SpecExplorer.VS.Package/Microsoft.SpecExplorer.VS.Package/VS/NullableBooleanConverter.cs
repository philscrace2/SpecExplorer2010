using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.SpecExplorer.VS
{
	[ValueConversion(typeof(bool?), typeof(bool))]
	public class NullableBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return false;
			}
			bool flag = parameter == null || bool.Parse(parameter.ToString());
			return !((bool)value ^ flag);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = parameter == null || bool.Parse(parameter.ToString());
			return !((bool)value ^ flag);
		}
	}
}
