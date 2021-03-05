using SerilogPrototype.Extensions;

namespace SerilogPrototype.Formatters
{
	public class StructuredLoggingNameFormatter : NameFormatter
	{
		public StructuredLoggingNameFormatter(string separator)
			: base(separator)
		{
		}

		public new static INameFormatter Create(string separator) => new StructuredLoggingNameFormatter(separator);

		public override string SanitizeName(string name)
		{
			return base.SanitizeName(name.SanitizeStructuredLoggingName());
		}
	}
}
