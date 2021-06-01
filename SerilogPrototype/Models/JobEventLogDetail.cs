using System;

namespace SerilogPrototype.Models
{
	public class JobEventLogDetail
	{
		public string LogEventType { get; set; }
		public DateTime LogDate { get; set; }
		public string Message { get; set; }
	}
}
