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

				Log.Logger = new LoggerConfiguration()
					.ReadFrom.Configuration(config)
					.WriteTo.Map(DEVICE_ID_PROPERTY_NAME, /*"_general",*/ (name, wt) =>
					{
						wt.AmazonS3(
							path: "log.txt",
							bucketName: awsSettings.BucketURL,
							endpoint: RegionEndpoint.USEast1,
							awsAccessKeyId: awsSettings.AccessKeyId,
							awsSecretAccessKey: awsSettings.AccessKey,
							restrictedToMinimumLevel: LogEventLevel.Debug,
							outputTemplate: "{Timestamp:o} [{Level:u3}] {Message}{NewLine}{Exception}",
							bucketPath: $"DeviceLog/{name}",
							batchSizeLimit: int.MaxValue,
							batchingPeriod: TimeSpan.FromMinutes(5),
							eagerlyEmitFirstEvent: false,
							failureCallback: ex => Console.WriteLine($"An error occured in AmazonS3 sink: {ex.Message}")
						);
					})
					.CreateLogger();

				for (var x = 0; x < 200; x++)
				{
					var ex = new Exception($"Test - {x}");
					Log.Logger.Error(ex, $"{{{DEVICE_ID_PROPERTY_NAME}}}", DEVICE_ID);
				}

				await EmulateWorkAsync();

				// without DeviceId
				Log.Logger.Information("This is Information without device ID");
				Log.Logger.Error("This is Error without device ID");

				// DeviceId = "111222777"
				Log.Logger.Information($"{{{DEVICE_ID_PROPERTY_NAME}}}. This is Information", "111222777");
				Log.Logger.Debug($"{{{DEVICE_ID_PROPERTY_NAME}}}. This is Debug", "111222777");
				Log.Logger.Error($"{{{DEVICE_ID_PROPERTY_NAME}}}. This is Error", "111222777");

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
