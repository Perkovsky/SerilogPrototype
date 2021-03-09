using System;

namespace SerilogPrototype.Models.StructuredLogging
{
	public class JobEventLogStructuredLogging
	{
		public int JobId { get; set; }
		public string EventId { get; set; }
		public DateTime Date { get; set; }
		public string Status { get; set; }
		public string TaskName { get; set; }
		public string Type { get; set; }
		public string Message { get; set; }
	}
}
