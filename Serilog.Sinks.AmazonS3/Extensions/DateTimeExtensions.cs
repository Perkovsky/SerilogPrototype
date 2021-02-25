using System;

namespace Serilog
{
	public static class DateTimeExtensions
	{
		public static string ToTimeStamp(this DateTime date, string seperator, string appendString)
		{
			return date.ToString($"yyyy{seperator}MM{seperator}dd{seperator}HH{seperator}mm{seperator}ss") + appendString;
		}

		public static string ToTimeStamp(this DateTime date) => date.ToTimeStamp("", "");
	}
}
