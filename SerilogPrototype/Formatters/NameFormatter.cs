using System;
using System.Text.RegularExpressions;

namespace SerilogPrototype.Formatters
{
    public class NameFormatter : INameFormatter
    {
        private static readonly Regex _pattern = new Regex("(?<=[a-z0-9])[A-Z]", RegexOptions.Compiled);
        private readonly string _separator;

        public NameFormatter(string separator)
        {
            if (string.IsNullOrWhiteSpace(separator))
                throw new ArgumentException("Value cannot be null or empty.", nameof(separator));

            _separator = separator;
        }

        public static INameFormatter Create(string separator) => new NameFormatter(separator);

        public virtual string SanitizeName(string name)
        {
            return _pattern.Replace(name, m => _separator + m.Value).ToLowerInvariant();
        }
    }
}
