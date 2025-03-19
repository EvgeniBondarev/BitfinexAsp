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
    private readonly BitfinexWebSocketTradesClient _webSocketClient;
    private readonly BitfinexWebSocketCandlesClient _webSocketCandlesClient;
    private readonly ConcurrentDictionary<string, bool> _subscriptions = new();
    private readonly ConcurrentDictionary<string, bool> _subscriptions2 = new();

    public BitfinexConnector(BitfinexClient bitfinexClient,
                             BitfinexWebSocketTradesClient webSocketClient,
                             BitfinexWebSocketCandlesClient webSocketCandlesClient)
    {
        _bitfinexClient = bitfinexClient;
        _webSocketClient = webSocketClient;
        _webSocketClient.OnTradeReceived += HandleTrade;
        
        _webSocketCandlesClient = webSocketCandlesClient;
        _webSocketCandlesClient.OnCandleReceived += HandleCandle;
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

    public void SubscribeCandles(string pair, int periodInSec, long? count, DateTimeOffset? from = null, DateTimeOffset? to = null)
    {
        string key = $"trade:{TimeConvertor.ConvertSecondsToMinutes(periodInSec)}m:{pair}";

        if (!_subscriptions2.ContainsKey(key))
        {
            _subscriptions2[key] = true;
            _webSocketCandlesClient.SubscribeCandles(pair, periodInSec);
        }
    }

    public void UnsubscribeCandles(string pair)
    {
        string key = $"trade:*m:{pair}";
        var keysToUnsubscribe = _subscriptions.Keys.Where(k => k.Contains(key)).ToList();

        foreach (var k in keysToUnsubscribe)
        {
            _subscriptions2.TryRemove(k, out _);
            int periodInSec = int.Parse(k.Split(':')[1].TrimEnd('m')) * 60;
            _webSocketCandlesClient.UnsubscribeCandles(pair, periodInSec);
        }
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
    private void HandleCandle(Candle candle)
    {
        CandleSeriesProcessing?.Invoke(candle);
    }
}