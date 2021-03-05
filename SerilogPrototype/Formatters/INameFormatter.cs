namespace SerilogPrototype.Formatters
{
	public interface INameFormatter
	{
		/// <summary>
		/// Clean up a name so that it matches the formatting.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Examples:
		/// </para>
		/// <code>"MgLoggerIndex" -> "mg-logger-index" (kebab case)</code>
		/// <code>"MgLoggerIndex" -> "mg_logger_index" (snake case)</code>
		/// </remarks>
		/// <param name="name"></param>
		/// <returns>string</returns>
		string SanitizeName(string name);
	}
}
