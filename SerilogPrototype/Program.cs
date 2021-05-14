using Microsoft.Extensions.Configuration;
using Serilog;
using SerilogPrototype.Extensions;
using SerilogPrototype.Models;
using SerilogPrototype.Models.StructuredLogging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SerilogPrototype
{
	class Program
	{
		const string LOG_NAME = "reproduce-429";
		const int LENGTH = 100000;

		static int errors = 0;

		static void AddSystemLogStructuredLogging(int id)
		{
			// StructuredLogging
			Log.Logger.Information<SystemLogStructuredLogging>(new SystemLogStructuredLogging
			{
				EntityId = id,
				Type = "SerilogPrototype.Models.StructuredLogging.SystemLogStructuredLogging",
				Created = DateTime.Now,
				Component = $"This is my Component #{id}",
				Description = "Just some text"
			});
		}

		static void FillLogUsingParallel()
		{
			Parallel.For(0, LENGTH, i => AddSystemLogStructuredLogging(i));
		}

		static void FillLogUsingTaskRun()
		{
			for (int i = 0; i < LENGTH; i++)
			{
				Task.Run(() => AddSystemLogStructuredLogging(i));
			}
		}

		static async Task FillLogUsingWhenAll()
		{
			var tasks = new List<Task>();
			for (int i = 0; i < LENGTH; i++)
			{
				tasks.Add(Task.Run(() => AddSystemLogStructuredLogging(i)));
			}
			await Task.WhenAll(tasks);
		}

		static async Task FillLogAsync()
		{
			for (int i = 0; i < LENGTH; i++)
			{
				AddSystemLogStructuredLogging(i);
			}
			await Task.CompletedTask;
		}

		static void FillLog()
		{
			for (int i = 0; i < LENGTH; i++)
			{
				AddSystemLogStructuredLogging(i);
			}
		}

		static void Main(string[] args)
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
					//.ReadFrom.Configuration(config)
					.WriteToElasticsearch(awsSettings, LOG_NAME)
					.CreateLogger();

				Serilog.Debugging.SelfLog.Enable(msg =>
				{
					////Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
					//var file = File.CreateText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"serilog_selfLog_{DateTime.UtcNow.ToString($"yyyyMMddHHmmssfffffff")}.txt"));
					//Serilog.Debugging.SelfLog.Enable(TextWriter.Synchronized(file));

					if (!string.IsNullOrWhiteSpace(msg))
					{
						Console.WriteLine(msg);
						Console.WriteLine($"-----> Errors: {++errors}");
					}
				});

				var sw = new Stopwatch();
				sw.Start();

				FillLog();
				//FillLogUsingParallel();
				//await FillLogUsingWhenAll();
				//FillLogUsingTaskRun();
				//_ = FillLogAsync();

				sw.Stop();
				Console.WriteLine(new string('-', 20));
				Console.WriteLine($"Spent time : {sw.Elapsed}");
				Console.WriteLine($"Errors	   : {errors}");
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
