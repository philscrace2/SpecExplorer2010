using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public class ItemWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double num = ((parameter != null) ? double.Parse(parameter.ToString()) : 1.0);
			return (double)value * num;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
