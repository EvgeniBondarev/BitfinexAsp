using System.Collections.Concurrent;
using BitfinexAsp.ApiClients.Bitfinex.REST;
using BitfinexAsp.ApiClients.Bitfinex.WebSocket;
using BitfinexAsp.Models;
using BitfinexAsp.Models.JsonToModelConverter.Converters;
using BitfinexAsp.Utils;

namespace BitfinexAsp.ApiClients.Connectors.Implementation;

public class BitfinexConnector : ITestConnector
{
    private readonly BitfinexClient _bitfinexClient;
    private readonly BitfinexWebSocketClient _webSocketClient;
    private readonly ConcurrentDictionary<string, bool> _subscriptions = new();

    public BitfinexConnector(BitfinexClient bitfinexClient,
                             BitfinexWebSocketClient webSocketClient)
    {
        _bitfinexClient = bitfinexClient;
        _webSocketClient = webSocketClient;
        _webSocketClient.OnTradeReceived += HandleTrade;
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
        if (_subscriptions.ContainsKey(pair)) return;
        _subscriptions[pair] = true;
        _ = _webSocketClient.ConnectAsync(pair);
    }

    public void UnsubscribeTrades(string pair)
    {
        if (_subscriptions.TryRemove(pair, out _))
        {
            _ = _webSocketClient.DisconnectAsync();
        }
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
    
    private void HandleTrade(Trade trade) // Возможно NewBuyTrade и NewSellTrade про другое
    {
        if (trade.Side == "buy")
        {
            NewBuyTrade?.Invoke(trade);
        }
        else if (trade.Side == "sell")
        {
            NewSellTrade?.Invoke(trade);
        }
    }
}