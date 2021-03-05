using System;

namespace SerilogPrototype.Models.StructuredLogging
{
	public class TransactionLogStructuredLogging : IStructuredLogging
	{
		public string BatchId { get; set; }
		public string Message { get; set; }
		public DateTime CreateDate { get; set; }
		public int? AchFileId { get; set; }
		public int? DistributionLineId { get; set; }
		public int? TenantTransactionId { get; set; }
	}
}
