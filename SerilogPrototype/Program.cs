using Amazon;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using SerilogPrototype.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SerilogPrototype
{
	class Program
	{
		public const string DEVICE_ID_PROPERTY_NAME = "DeviceId";
		public const string DEVICE_ID = "www-44-mmm";

		static async Task EmulateWorkAsync()
		{
			await Task.CompletedTask;
			Log.Logger.Information($"{{{DEVICE_ID_PROPERTY_NAME}}}. {nameof(EmulateWorkAsync)} has been called.", DEVICE_ID);
		}

		static async Task Main(string[] args)
		{
			try
			{
				Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

				var config = new ConfigurationBuilder()
				.AddUserSecrets<AWSSettings>()
				.SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location))
				.AddJsonFile("appsettings.json")
				.Build();

				var awsSettings = config.GetSection("AWSSettings").Get<AWSSettings>();

				//AmazonS3(string path, string bucketName, RegionEndpoint endpoint, string awsAccessKeyId, string awsSecretAccessKey, bool autoUploadEvents = false, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose, string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", IFormatProvider formatProvider = null, long ? fileSizeLimitBytes = 1073741824, LoggingLevelSwitch levelSwitch = null, bool buffered = false, RollingInterval rollingInterval = RollingInterval.Day, int ? retainedFileCountLimit = 31, Encoding encoding = null, FileLifecycleHooks hooks = null, Action < Exception > failureCallback = null, string bucketPath = null);

				//	target.WriteTo.AmazonS3(
				//		path: "log.txt",
				//		bucketName: awsSettings.Bucket,
				//		endpoint: RegionEndpoint.USEast1,
				//		awsAccessKeyId: awsSettings.AccessKeyId,
				//		awsSecretAccessKey: awsSettings.AccessKey,
				//		restrictedToMinimumLevel: LogEventLevel.Debug,
				//		outputTemplate: "{Timestamp:o} [{Level:u3}] {Message}{NewLine}{Exception}",
				//		retainedFileCountLimit: null,
				//		//fileSizeLimitBytes: 1048576, // 1mb
				//		//rollingInterval: RollingInterval.Minute,
				//		//buffered: true,
				//		autoUploadEvents: true,
				//		//levelSwitch: levelSwitch,
				//		//encoding: Encoding.Unicode,
				//		//hooks: new HeaderWriter("Timestamp,Level,Message"),
				//		//bucketPath = "awsSubPath",
				//		failureCallback: e => Console.WriteLine($"An error occured in AmazonS3 sink: {e.Message}")
				//	);


				Log.Logger = new LoggerConfiguration()
					.ReadFrom.Configuration(config)
					.WriteTo.Map(DEVICE_ID_PROPERTY_NAME, "_general", (name, wt) =>
					{
						//wt.File(
						//	path: $@"{BASE_LOG_FOLDER}\{name}.txt",
						//	outputTemplate: outputTemplate
						//);
						//wt.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchEndpoint))
						//{
						//	AutoRegisterTemplate = true,
						//	OverwriteTemplate = true,
						//	DetectElasticsearchVersion = true,
						//	IndexFormat = $"mg-{name}-index-{{0:yyyy.MM}}",
						//	EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog,
						//	RegisterTemplateFailure = RegisterTemplateRecovery.IndexAnyway
						//});

						//sb.Append($"{now.Year}/{now.Month:#00}/{now.Day:#00}");


						wt.AmazonS3(
							path: "log.txt",
							bucketName: awsSettings.BucketURL,
							endpoint: RegionEndpoint.USEast1,
							awsAccessKeyId: awsSettings.AccessKeyId,
							awsSecretAccessKey: awsSettings.AccessKey,
							restrictedToMinimumLevel: LogEventLevel.Debug,
							outputTemplate: "{Timestamp:o} [{Level:u3}] {Message}{NewLine}{Exception}",
							bucketPath: $"DeviceLog/{name}",
							//rollingInterval: Serilog.Sinks.AmazonS3.RollingInterval.Minute,
							batchSizeLimit: int.MaxValue,
							batchingPeriod: TimeSpan.FromMinutes(5),
							eagerlyEmitFirstEvent: false,
							failureCallback: ex => Console.WriteLine($"An error occured in AmazonS3 sink: {ex.Message}")
						);
					})
					//.WriteTo.AmazonS3(
					//	path: "log.txt",
					//	bucketName: awsSettings.BucketURL,
					//	endpoint: RegionEndpoint.USEast1,
					//	awsAccessKeyId: awsSettings.AccessKeyId,
					//	awsSecretAccessKey: awsSettings.AccessKey,
					//	restrictedToMinimumLevel: LogEventLevel.Debug,
					//	outputTemplate: "{Timestamp:o} [{Level:u3}] {Message}{NewLine}{Exception}",
					//	//autoUploadEvents: true, // very looong work
					//	//fileSizeLimitBytes: 5242880, //5mb		// 1048576, // 1mb
					//	//buffered: true,
					//	//new CultureInfo("de-DE"),
					//	//levelSwitch: levelSwitch,
					//	//retainedFileCountLimit: 10,
					//	//encoding: Encoding.Unicode,
					//	//hooks: new HeaderWriter("Timestamp,Level,Message"),
					//	bucketPath: "DeviceLog/www-44-mmm",
					//	//rollingInterval: Serilog.Sinks.AmazonS3.RollingInterval.Minute,
					//	batchSizeLimit: int.MaxValue,
					//	batchingPeriod: TimeSpan.FromMinutes(3),
					//	eagerlyEmitFirstEvent: false,
					//	failureCallback: ex => Console.WriteLine($"An error occured in AmazonS3 sink: {ex.Message}")
					//)
					.CreateLogger();

				for (var x = 0; x < 200; x++)
				{
					var ex = new Exception($"Test - {x}");
					Log.Logger.Error($"{{{DEVICE_ID_PROPERTY_NAME}}}. {ex.ToString()}", DEVICE_ID);
				}

				await EmulateWorkAsync();

				Log.Logger.Information("This is Information without device ID");
				Log.Logger.Error("This is Error without device ID");

				Log.Logger.Information($"{{{DEVICE_ID_PROPERTY_NAME}}}. This is Information", DEVICE_ID);
				Log.Logger.Debug($"{{{DEVICE_ID_PROPERTY_NAME}}}. This is Debug", DEVICE_ID);
				Log.Logger.Error($"{{{DEVICE_ID_PROPERTY_NAME}}}. This is Error", DEVICE_ID);
				Log.Information($"{{{DEVICE_ID_PROPERTY_NAME}}}. Information from console app!", DEVICE_ID);
				Log.Error($"{{{DEVICE_ID_PROPERTY_NAME}}}. Error from console app!", DEVICE_ID);
				Log.Logger.Fatal($"{{{DEVICE_ID_PROPERTY_NAME}}}. This is Fatal", DEVICE_ID);
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, $"{{{DEVICE_ID_PROPERTY_NAME}}}. {typeof(Program).Namespace}: {nameof(Main)} caught an error.", DEVICE_ID);
			}
			{
				Console.WriteLine($"{Environment.NewLine}Press any key...");
				Console.ReadKey();
				Log.CloseAndFlush();
			}
		}
	}
}
