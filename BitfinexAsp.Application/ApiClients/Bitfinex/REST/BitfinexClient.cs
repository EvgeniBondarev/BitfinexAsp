using BitfinexAsp.Models;
using BitfinexAsp.Models.JsonToModelConverter;
using BitfinexAsp.Models.JsonToModelConverter.Converters;
using Newtonsoft.Json;

namespace BitfinexAsp.ApiClients.Bitfinex.REST;

public class BitfinexClient
{
    private readonly MainClient _apiClient;
    
    public BitfinexClient(MainClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<Trade>> GetTradesAsync(string symbol, int limit)
    {
        string endpoint = BitfinexEndpoints.GetTrades(symbol, limit);
        string jsonResponse = await _apiClient.SendRequestAsync(HttpMethod.Get, endpoint);
        return JsonConvert.DeserializeObject<List<Trade>>(jsonResponse, Converter.Settings);
    }

    public async Task<List<Candle>> GetCandlesAsync(string candle, 
                                                    string section, 
                                                    DateTimeOffset? start = null, 
                                                    DateTimeOffset? end = null, 
                                                    long? limit = null)
    {
        string endpoint = BitfinexEndpoints.GetCandles(candle, section, start?.ToUnixTimeMilliseconds(), end?.ToUnixTimeMilliseconds(), limit);
        string jsonResponse = await _apiClient.SendRequestAsync(HttpMethod.Get, endpoint);
        return JsonConvert.DeserializeObject<List<Candle>>(jsonResponse, Converter.Settings);
    }
    
    public async Task<Ticker> GetTickerAsync(string symbol)
    {
        string endpoint = BitfinexEndpoints.GetTicker(symbol);
        string jsonResponse = await _apiClient.SendRequestAsync(HttpMethod.Get, endpoint);
        return JsonConvert.DeserializeObject<Ticker>(jsonResponse, Converter.Settings);
    }
}