using Amazon;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using SerilogPrototype.Models;
using System;

namespace SerilogPrototype.Extensions
{
	public static class LoggerConfigurationExtensions
	{
		public static LoggerConfiguration WriteToAmazonS3(this LoggerConfiguration target, AWSSettings awsSettings, string logName)
		{
			return target.WriteTo.AmazonS3(
				path: "log.txt",
				bucketName: awsSettings.LogBucketUrl,
				endpoint: RegionEndpoint.USEast1,
				awsAccessKeyId: awsSettings.AccessKeyId,
				awsSecretAccessKey: awsSettings.AccessKey,
				restrictedToMinimumLevel: LogEventLevel.Debug,
				outputTemplate: "{Timestamp:o} [{Level:u3}] {Message}{NewLine}{Exception}",
				bucketPath: $"{logName}",
				batchSizeLimit: int.MaxValue,
				batchingPeriod: TimeSpan.FromMinutes(5),
				eagerlyEmitFirstEvent: false,
				failureCallback: ex => Console.WriteLine($"An error occured in AmazonS3 sink: {ex.Message}")
			);
		}

		public static LoggerConfiguration WriteToElasticsearch(this LoggerConfiguration target, AWSSettings awsSettings, string logName)
		{
			return target.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(awsSettings.ElasticsearchUrl))
			{
				AutoRegisterTemplate = true,
				OverwriteTemplate = true,
				DetectElasticsearchVersion = true,
				IndexFormat = $"mg-log-{logName}-{{0:yyyy.MM}}",
				RegisterTemplateFailure = RegisterTemplateRecovery.IndexAnyway,
				EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog | EmitEventFailureHandling.RaiseCallback,
				FailureCallback = e => Console.WriteLine($"An error occured in Elasticsearch sink: {e.MessageTemplate}")
			});
		}
	}
}
