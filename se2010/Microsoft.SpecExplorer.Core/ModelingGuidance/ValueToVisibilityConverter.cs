using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public class ValueToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = parameter == null || bool.Parse(parameter.ToString());
			bool flag2 = ((value is bool) ? ((bool)value) : ((!(value is int)) ? (value != null) : ((int)value != 0)));
			return (!(flag ? flag2 : (!flag2))) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
