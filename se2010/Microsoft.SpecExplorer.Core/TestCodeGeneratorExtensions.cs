namespace Microsoft.SpecExplorer
{
	public static class TestCodeGeneratorExtensions
	{
		public static bool IsNoneOrEmptyValue(this string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}
			return string.Compare(value, "none", true) == 0;
		}
	}
}
