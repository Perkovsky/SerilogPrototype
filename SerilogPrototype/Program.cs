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
		static async Task EmulateWorkAsync()
		{
			await Task.CompletedTask;
			Log.Logger.Information($"{nameof(EmulateWorkAsync)} has been called.");
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

				Log.Logger = new LoggerConfiguration()
					.ReadFrom.Configuration(config)
					.WriteTo.AmazonS3(
						path: "log.txt",
						bucketName: awsSettings.BucketURL,
						endpoint: RegionEndpoint.USEast1,
						awsAccessKeyId: awsSettings.AccessKeyId,
						awsSecretAccessKey: awsSettings.AccessKey,
						restrictedToMinimumLevel: LogEventLevel.Debug,
						outputTemplate: "{Timestamp:o} [{Level:u3}] {Message}{NewLine}{Exception}",
						//autoUploadEvents: true, // very looong work
						fileSizeLimitBytes: 200, // 1048576, // 1mb
						//buffered: true,
						//new CultureInfo("de-DE"),
						//levelSwitch: levelSwitch,
						rollingInterval: Serilog.Sinks.AmazonS3.RollingInterval.Minute,
						//retainedFileCountLimit: 10,
						//encoding: Encoding.Unicode,
						//hooks: new HeaderWriter("Timestamp,Level,Message"),
						//bucketPath = "awsSubPath",
						failureCallback: ex => Console.WriteLine($"An error occured in AmazonS3 sink: {ex.Message}")
					)
					.CreateLogger();

				for (var x = 0; x < 200; x++)
				{
					var ex = new Exception($"Test - {x}");
					Log.Logger.Error(ex.ToString());
				}

				await EmulateWorkAsync();

				Log.Logger.Information("This is Information");
				Log.Logger.Debug("This is Debug");
				Log.Logger.Error("This is Error");
				Log.Information("Information from console app!");
				Log.Error("Error from console app!");
				Log.Logger.Fatal("This is Fatal");
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, $"{typeof(Program).Namespace}: {nameof(Main)} caught an error.");
			}
			{
				Log.CloseAndFlush();
			}

			Console.WriteLine($"{Environment.NewLine}Press any key...");
			Console.ReadKey();
		}
	}
}
