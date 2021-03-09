using Newtonsoft.Json;
using Serilog;

namespace SerilogPrototype.Extensions
{
	public static class LoggerExtensions
	{
		/// <summary>
		/// Write a structured log event with the Serilog.Events.LogEventLevel.Information level.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="logger">The globally-shared logger.</param>
		/// <param name="structuredLogging">The structured logging data.</param>
		public static void Information<T>(this ILogger logger, T structuredLogging)
			where T : class
		{
			var structuredLoggingTypeName = typeof(T).Name.SanitizeStructuredLoggingName();
			logger.ForContext(structuredLoggingTypeName, structuredLogging, true)
				.Information(JsonConvert.SerializeObject(structuredLogging));
		}
	}
}
