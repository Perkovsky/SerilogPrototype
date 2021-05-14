using Amazon;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using SerilogPrototype.Models;
using System;
using System.Collections.Specialized;

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
			//var headers = new NameValueCollection();
			//headers.Add("Host", "vpc-mg-uat-es01-oztvy3bcj5rg7prtnlywny2sv4.us-east-1.es.amazonaws.com");

			return target.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(awsSettings.ElasticsearchUrl))
			{
				//BatchPostingLimit = 5000, //int.MaxValue,
				//Period = TimeSpan.FromMinutes(5),

				ModifyConnectionSettings = x =>
				{
					return x.ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true)
						//.GlobalHeaders(headers)					// just for test on UAT
						.MaxRetryTimeout(TimeSpan.FromMinutes(5))   // just for test on UAT
						.RequestTimeout(TimeSpan.FromMinutes(5))    // just for test on UAT
						//.PingTimeout(TimeSpan.FromSeconds(10))	// just for test on UAT
						;
				},
				//AutoRegisterTemplate = true,
				//AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
				//TemplateName = $"mg-{logName}-template",
				//OverwriteTemplate = true,
				//RegisterTemplateFailure = RegisterTemplateRecovery.IndexAnyway,
				DetectElasticsearchVersion = true,
				IndexFormat = $"mg-{logName}-{{0:yyyy.MM}}",
				EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog | EmitEventFailureHandling.WriteToFailureSink | EmitEventFailureHandling.RaiseCallback,
				//FailureSink = failureSink,
				//FailureCallback = e => failureCallback(new Exception($"Elasticsearch Sink has thrown an exception! Use debug mode for more information. MessageTemplate: {e.MessageTemplate}"))
			});
		}
	}
}
