using Microsoft.Extensions.Configuration;
using Serilog;
using SerilogPrototype.Extensions;
using SerilogPrototype.Models;
using SerilogPrototype.Models.StructuredLogging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SerilogPrototype
{
	class Program
	{
		public const string LOG_NAME = "serilog-prototype";

		static async Task EmulateWorkAsync()
		{
			await Task.CompletedTask;
			Log.Logger.Information($"{nameof(EmulateWorkAsync)} has been called.");
		}

		static void LogInnerException()
		{
			var argumentOutOfRange = new ArgumentOutOfRangeException("Arg. Out Of range");
			var accessViolation = new AccessViolationException("Access exceprion error", argumentOutOfRange);
			var appDomain = new AppDomainUnloadedException("AppDomain bla-bla-bla", accessViolation);
			var internalBufferOverflow = new InternalBufferOverflowException("Internal---Buffer---Overflow", appDomain);
			var exception = new Exception("Just test inner exceptions", internalBufferOverflow);
			//string result = exception.ToString();
			//Console.WriteLine(result);
			Log.Logger.Error(exception, $"{nameof(LogInnerException)} has been called.");
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
					.WriteToAmazonS3(awsSettings, LOG_NAME)
					.WriteToElasticsearch(awsSettings, LOG_NAME)
					.CreateLogger();

				for (var x = 0; x < 200; x++)
				{
					var ex = new Exception($"Test - {x}");
					Log.Logger.Error(ex, "This is Error from cycle!");
				}

				await EmulateWorkAsync();

				Log.Logger.Information("Information from console app!");
				Log.Logger.Debug("This is Debug from console app!");
				Log.Logger.Error("This is Error from console app!");
				Log.Logger.Fatal("This is Fatal from console app!");

				// StructuredLogging
				Log.Logger.Information<SystemLogStructuredLogging>(new SystemLogStructuredLogging
				{
					EntityId = 2235,
					Type = "SerilogPrototype.Models.StructuredLogging.SystemLogStructuredLogging",
					Created = DateTime.Now,
					Component = "This is my Component",
					Description = "Just some text"
				});
				Log.Logger.Information<TransactionLogStructuredLogging>(new TransactionLogStructuredLogging
				{
					BatchId = "1135-5882-2323-8914",
					TenantTransactionId = 342353,
					CreateDate = DateTime.Now,
					Message = "Just some message"
				});

				LogInnerException();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "{typeof(Program).Namespace}: {nameof(Main)} caught an error.");
			}
			finally
			{
				Console.WriteLine($"{Environment.NewLine}Press any key...");
				Console.ReadKey();
				Log.CloseAndFlush();
			}
		}
	}
}
