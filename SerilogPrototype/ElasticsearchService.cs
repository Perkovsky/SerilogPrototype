using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using Serilog.Sinks.Elasticsearch.Durable;
using SerilogPrototype.Models.StructuredLogging;

namespace SerilogPrototype
{
	public class ElasticsearchService
	{
		private static int errors = 0;

		private readonly string _logName;

		private string _indexName = "mg-reproduce-429-2021.05";

		protected int DefaultHitSize => 10000;
		protected string IndexTemplate => $"mg-{_logName}-*";
		protected IElasticClient ElasticClient { get; private set; }
		//protected ElasticLowLevelClient ElasticClient { get; private set; }

		public ElasticsearchService(string endpoint, string logName)
		{
			_logName = logName;

			// init Elacticsearch
			var settings = new ConnectionSettings(new Uri(endpoint));
			settings.ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);
			settings.ThrowExceptions(alwaysThrow: true);
			settings.DisableDirectStreaming();
			settings.DefaultIndex(_indexName);
			settings.PrettyJson(); // good for DEBUG
			ElasticClient = new ElasticClient(settings);


			//// init Elacticsearch
			//var configuration = new ConnectionConfiguration(options.ConnectionPool, options.Connection, options.Serializer)
			//	.RequestTimeout(options.ConnectionTimeout);

			//configuration = options.ModifyConnectionSettings(configuration);
			//configuration.ThrowExceptions();
			//ElasticClient = new ElasticLowLevelClient(configuration);
		}

		private async Task<bool> IsIndexExistsAsync()
		{
			var result = await ElasticClient.Indices.ExistsAsync(IndexTemplate);
			return result.Exists;
		}

		public async Task DeleteIndexAsync()
		{
			if (await IsIndexExistsAsync())
				await ElasticClient.Indices.DeleteAsync(IndexTemplate);
		}

		private async Task SendPayloadAsync<T>(IEnumerable<T> payload)
			where T : class
		{
			try
			{
				var response = await ElasticClient.BulkAsync(b => b
					.Index(_indexName)
					.IndexMany(payload)
				);

				//var response = await ElasticClient.IndexManyAsync(payload);

				if (response.Errors)
				{
					foreach (var itemWithError in response.ItemsWithErrors)
					{
						Console.WriteLine($"Failed to index document {itemWithError.Id}: {itemWithError.Error}");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.WriteLine($"-----> Errors: {++errors}");
			}
		}

		private SystemLogStructuredLogging GetSystemLogStructuredLogging(int id)
		{
			return new SystemLogStructuredLogging
			{
				EntityId = id,
				Type = "SerilogPrototype.Models.StructuredLogging.SystemLogStructuredLogging",
				Created = DateTime.Now,
				Component = $"This is my Component #{id}",
				Description = "Just some text"
			};
		}

		private async Task<CreateIndexResponse> CreateIndexAsync()
		{
			var result = await ElasticClient.Indices.CreateAsync(_indexName, c => c
				.Map<SystemLogStructuredLogging>(mm => mm.AutoMap())
			);

			if (!result.IsValid)
				Console.WriteLine(result.ServerError.Error);

			return result;
		}

		public async Task AddSystemLogStructuredLoggingAsync()
		{
			await DeleteIndexAsync();
			await CreateIndexAsync();

			int batchPostingLimit = 5000;
			int length = 100000;
			var payload = Enumerable.Range(0, batchPostingLimit)
				.Select(n => GetSystemLogStructuredLogging(n));

			//for (int i = 0; i < length / batchPostingLimit; i++)
			//{
			//	await SendPayloadAsync(payload);
			//}

			Parallel.For(0, length / batchPostingLimit, new ParallelOptions { MaxDegreeOfParallelism = 3 }, i => _ = SendPayloadAsync(payload));
		}
	}
}
