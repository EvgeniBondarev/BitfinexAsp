using BitfinexAsp.ApiClients.Bitfinex.REST;
using BitfinexAsp.Models;
using BitfinexAsp.Models.JsonToModelConverter.Converters;
using BitfinexAsp.Utils;

namespace BitfinexAsp.ApiClients.Connectors.Implementation;

public class BitfinexConnector : ITestConnector
{
    private readonly MainClient _apiClient;
    private readonly BitfinexClient _bitfinexClient;

    public BitfinexConnector(MainClient apiClient, BitfinexClient bitfinexClient)
    {
        _apiClient = apiClient;
        _bitfinexClient = bitfinexClient;
    }
    
    public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
    {
        return await _bitfinexClient.GetTradesAsync(pair, maxCount);
    }

    public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from,
                                                                long? count, DateTimeOffset? to = null)
    {
        string candle = $"trade:{TimeConvertor.ConvertSecondsToMinutes(periodInSec)}m:{pair}";
        return await _bitfinexClient.GetCandlesAsync(candle, "hist", from, to, count);
    }
    
    public async Task<Ticker> GetTickerAsync(string symbol)
    {
        return await _bitfinexClient.GetTickerAsync(symbol);
    }

    public event Action<Trade>? NewBuyTrade;
    public event Action<Trade>? NewSellTrade;
    public void SubscribeTrades(string pair, int maxCount = 100)
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeTrades(string pair)
    {
        throw new NotImplementedException();
    }

    public event Action<Candle>? CandleSeriesProcessing;

    public void SubscribeCandles(string pair, int periodInSec, long? count,
                                DateTimeOffset? from = null, DateTimeOffset? to = null)
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeCandles(string pair)
    {
        throw new NotImplementedException();
    }
}
public class HttpClientFactoryStub : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => new HttpClient();
}