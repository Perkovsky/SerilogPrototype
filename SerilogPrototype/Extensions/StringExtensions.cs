using SerilogPrototype.Formatters;

namespace SerilogPrototype.Extensions
{
	public static class StringExtensions
	{
		public static string ToKebabCase(this string target)
		{
			return NameFormatter.Create("-").SanitizeName(target);
		}

		public static string ToSnakeCase(this string target)
		{
			return NameFormatter.Create("_").SanitizeName(target);
		}

		public static string SanitizeStructuredLoggingName(this string target)
		{
			const string STRUCTURED_LOGGING = "StructuredLogging";

			return target.EndsWith(STRUCTURED_LOGGING)
				? target.Substring(0, target.Length - STRUCTURED_LOGGING.Length)
				: target;
		}
	}
}
