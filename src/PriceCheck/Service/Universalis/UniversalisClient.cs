// ReSharper disable ConvertIfStatementToReturnStatement

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PriceCheck
{
	public class UniversalisClient : IUniversalisClient
	{
		private const string Endpoint = "https://universalis.app/api/";
		private readonly HttpClient _httpClient;
		private readonly IPluginWrapper _plugin;

		public UniversalisClient(IPluginWrapper plugin)
		{
			_plugin = plugin;
			_httpClient = new HttpClient
			{
				Timeout = TimeSpan.FromMilliseconds(plugin.GetConfig().RequestTimeout)
			};
		}

		public MarketBoardData GetMarketBoard(uint? worldId, ulong itemId)
		{
			var marketBoardFromAPI = GetMarketBoardData(worldId, itemId);
			return marketBoardFromAPI;
		}

		public void Dispose()
		{
			_httpClient.Dispose();
		}

		private MarketBoardData GetMarketBoardData(uint? worldId, ulong itemId)
		{
			HttpResponseMessage result;
			try
			{
				result = GetMarketBoardDataAsync(worldId, itemId).Result;
			}
			catch (Exception ex)
			{
				_plugin.LogError(ex,
					"Failed to retrieve data from Universalis for itemId {0} / worldId {1}.",
					itemId, worldId);
				return null;
			}

			if (result.StatusCode != HttpStatusCode.OK)
			{
				_plugin.LogError(
					"Failed to retrieve data from Universalis for itemId {0} / worldId {1} with HttpStatusCode {2}.",
					itemId, worldId, result.StatusCode);
				return null;
			}

			var json = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
			if (json == null)
			{
				_plugin.LogError("Failed to deserialize Universalis response for itemId {0} / worldId {1}.",
					itemId, worldId);
				return null;
			}

			try
			{
				var marketBoardData = new MarketBoardData
				{
					LastCheckTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
					LastUploadTime = json.lastUploadTime?.Value,
					AveragePriceNQ = json.averagePriceNQ?.Value,
					AveragePriceHQ = json.averagePriceHQ?.Value
				};
				return marketBoardData;
			}
			catch (Exception ex)
			{
				_plugin.LogError(ex, "Failed to parse marketBoard data for itemId {0} / worldId {1}.",
					itemId, worldId);
				return null;
			}
		}

		private async Task<HttpResponseMessage> GetMarketBoardDataAsync(uint? worldId, ulong itemId)
		{
			return await _httpClient.GetAsync(new Uri(Endpoint + "/" + worldId + "/" + itemId));
		}
	}
}