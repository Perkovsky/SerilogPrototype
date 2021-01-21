using Amazon;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AmazonS3;
using SerilogPrototype.Models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SerilogPrototype
{
	class Program
	{
		static async Task EmulateWorkAsync()
		{
			await Task.CompletedTask;
		}

		static async Task Main(string[] args)
		{
			var config = new ConfigurationBuilder()
				.AddUserSecrets<AWSSettings>()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
			
			var awsSettings = config.GetSection("AWSSettings").Get<AWSSettings>();

			//AmazonS3(string path, string bucketName, RegionEndpoint endpoint, string awsAccessKeyId, string awsSecretAccessKey, bool autoUploadEvents = false, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose, string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", IFormatProvider formatProvider = null, long ? fileSizeLimitBytes = 1073741824, LoggingLevelSwitch levelSwitch = null, bool buffered = false, RollingInterval rollingInterval = RollingInterval.Day, int ? retainedFileCountLimit = 31, Encoding encoding = null, FileLifecycleHooks hooks = null, Action < Exception > failureCallback = null, string bucketPath = null);

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(config)
				//.WriteTo.AmazonS3(
				//	path: "log.txt",
				//	bucketName: awsSettings.BucketURL,
				//	endpoint: RegionEndpoint.EUWest2,
				//	awsAccessKeyId: awsSettings.AccessKeyId,
				//	awsSecretAccessKey: awsSettings.AccessKey,
				//	restrictedToMinimumLevel: LogEventLevel.Debug,
				//	outputTemplate: "{Timestamp:o} [{Level:u3}] {Message}{NewLine}{Exception}",
				//	//fileSizeLimitBytes: 200,
				//	//new CultureInfo("de-DE"),
				//	//levelSwitch: levelSwitch,
				//	//buffered: true,
				//	//rollingInterval: RollingInterval.Minute,
				//	//retainedFileCountLimit: 10,
				//	//encoding: Encoding.Unicode,
				//	//hooks: new HeaderWriter("Timestamp,Level,Message"),
				//	//failureCallback: e => Console.WriteLine($"An error occured in my sink: {e.Message}"),
				//	//bucketPath = "awsSubPath"
				//)
				.CreateLogger();

			await EmulateWorkAsync();

			Log.Information("Hello from console app!");

			Console.ReadKey();
		}
	}
}
