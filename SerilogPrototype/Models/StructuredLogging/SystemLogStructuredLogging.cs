using System;

namespace SerilogPrototype.Models.StructuredLogging
{
	public class SystemLogStructuredLogging
	{
		public DateTime Created { get; set; }
		public string Type { get; set; }
		public string Component { get; set; }
		public string Description { get; set; }
		public int? EntityId { get; set; }
	}
}
