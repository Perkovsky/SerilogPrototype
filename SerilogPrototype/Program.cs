using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using Serilog.Filters;
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
		static void CreateLogFiles(string folder, int fileCount = 1000, int contentRowCount = 1)
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

			var content = JsonConvert.SerializeObject(new JobEventLogDetail
			{
				LogDate = DateTime.Now,
				LogEventType = "Info",
				Message = "This is a simple text message."
			});

			for (int i = 0; i < fileCount; i++)
			{
				var filename = Path.Combine(folder, $"{DateTime.Now:yyyyMMddHHmmssfffffff}.json");
				using (var file = File.CreateText(filename))
				{
					for (int k = 0; k < contentRowCount; k++)
					{
						file.WriteLine(content);
					}
				}
			}
		}

		static void Main(string[] args)
		{
			try
			{
				var sw = new Stopwatch();
				sw.Start();

				//CreateLogFiles("Logs1000");
				CreateLogFiles("Logs100x100", 100, 100);

				sw.Stop();
				Console.WriteLine(new string('-', 20));
				Console.WriteLine($"Spent time : {sw.Elapsed}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
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
