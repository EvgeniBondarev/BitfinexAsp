using BitfinexAsp.ApiClients.Bitfinex.REST;
using BitfinexAsp.ApiClients.Bitfinex.WebSocket;
using BitfinexAsp.ApiClients.Connectors.Implementation;
using BitfinexAsp.Models;
using BitfinexAsp.Models.JsonToModelConverter.Converters;
using Moq;

namespace BitfinexAsp.Tests;

public class BitfinexConnectorIntegrationTests
{
    private readonly Mock<BitfinexClient> _bitfinexClientMock;
    private readonly Mock<BitfinexWebSocketTradesClient> _webSocketTradesMock;
    private readonly Mock<BitfinexWebSocketCandlesClient> _webSocketCandlesMock;
    private readonly BitfinexConnector _connector;

    public BitfinexConnectorIntegrationTests()
    {
        _bitfinexClientMock = new Mock<BitfinexClient>();
        _webSocketTradesMock = new Mock<BitfinexWebSocketTradesClient>();
        _webSocketCandlesMock = new Mock<BitfinexWebSocketCandlesClient>();

        _connector = new BitfinexConnector(
            _bitfinexClientMock.Object, 
            _webSocketTradesMock.Object, 
            _webSocketCandlesMock.Object);
    }

    [Fact]
    public async Task GetNewTradesAsync_ReturnsTrades()
    {
        // Arrange
        var expectedTrades = new List<Trade>
        {
            new() { Price = 50000, Amount = 0.1m, Side = "buy" },
            new() { Price = 49000, Amount = 0.2m, Side = "sell" }
        };
        _bitfinexClientMock
            .Setup(client => client.GetTradesAsync("BTCUSD", 2))
            .ReturnsAsync(expectedTrades);

        // Act
        var trades = await _connector.GetNewTradesAsync("BTCUSD", 2);

        // Assert
        Assert.NotNull(trades);
        Assert.Equal(2, trades.Count());
        Assert.Contains(trades, t => t.Price == 50000 && t.Side == "buy");
    }

    [Fact]
    public async Task GetCandleSeriesAsync_ReturnsCandles()
    {
        // Arrange
        var expectedCandles = new List<Candle>
        {
            new() { Time = DateTimeOffset.UtcNow, Open = 49000, Close = 49500, High = 50000, Low = 48500 },
            new() { Time = DateTimeOffset.UtcNow.AddMinutes(-5), Open = 50000, Close = 49000, High = 50500, Low = 48800 }
        };
        _bitfinexClientMock
            .Setup(client => client.GetCandlesAsync(It.IsAny<string>(), "hist", null, null, null))
            .ReturnsAsync(expectedCandles);

        // Act
        var candles = await _connector.GetCandleSeriesAsync("BTCUSD", 300, null, null);

        // Assert
        Assert.NotNull(candles);
        Assert.Equal(2, candles.Count());
        Assert.Contains(candles, c => c.Open == 49000 && c.Close == 49500);
    }

    [Fact]
    public async Task GetTickerAsync_ReturnsTicker()
    {
        // Arrange
        var expectedTicker = new Ticker { LastPrice = 50000 };
        _bitfinexClientMock
            .Setup(client => client.GetTickerAsync("BTCUSD"))
            .ReturnsAsync(expectedTicker);

        // Act
        var ticker = await _connector.GetTickerAsync("BTCUSD");

        // Assert
        Assert.NotNull(ticker);
        Assert.Equal(50000, ticker.LastPrice);
    }

    [Fact]
    public void SubscribeTrades_TriggersWebSocketConnection()
    {
        // Act
        _connector.SubscribeTrades("BTCUSD");

        // Assert
        _webSocketTradesMock.Verify(ws => ws.ConnectAsync("BTCUSD"), Times.Once);
    }

    [Fact]
    public void UnsubscribeTrades_TriggersWebSocketDisconnection()
    {
        // Arrange
        _connector.SubscribeTrades("BTCUSD");

        // Act
        _connector.UnsubscribeTrades("BTCUSD");

        // Assert
        _webSocketTradesMock.Verify(ws => ws.DisconnectAsync(), Times.Once);
    }

    [Fact]
    public void SubscribeCandles_TriggersWebSocketSubscription()
    {
        // Act
        _connector.SubscribeCandles("BTCUSD", 300, null);

        // Assert
        _webSocketCandlesMock.Verify(ws => ws.SubscribeCandles("BTCUSD", 300), Times.Once);
    }

    [Fact]
    public void UnsubscribeCandles_TriggersWebSocketUnsubscription()
    {
        // Arrange
        _connector.SubscribeCandles("BTCUSD", 300, null);

        // Act
        _connector.UnsubscribeCandles("BTCUSD");

        // Assert
        _webSocketCandlesMock.Verify(ws => ws.UnsubscribeCandles("BTCUSD", 300), Times.Once);
    }

    [Fact]
    public void HandleTrade_InvokesCorrectEvent()
    {
        // Arrange
        var buyTrade = new Trade { Side = "buy", Price = 50000m, Amount = 0.1m };
        var sellTrade = new Trade { Side = "sell", Price = 49000m, Amount = 0.2m };

        Trade? receivedBuyTrade = null;
        Trade? receivedSellTrade = null;

        _connector.NewBuyTrade += trade => receivedBuyTrade = trade;
        _connector.NewSellTrade += trade => receivedSellTrade = trade;

        // Act
        _webSocketTradesMock.Raise(ws => ws.OnTradeReceived += null, buyTrade);
        _webSocketTradesMock.Raise(ws => ws.OnTradeReceived += null, sellTrade);

        // Assert
        Assert.NotNull(receivedBuyTrade);
        Assert.NotNull(receivedSellTrade);
        Assert.Equal(50000m, receivedBuyTrade.Price);
        Assert.Equal(49000m, receivedSellTrade.Price);
    }

    [Fact]
    public void HandleCandle_InvokesCandleEvent()
    {
        // Arrange
        var candle = new Candle { Open = 49000, Close = 49500, High = 50000, Low = 48500 };
        Candle? receivedCandle = null;

        _connector.CandleSeriesProcessing += c => receivedCandle = c;

        // Act
        _webSocketCandlesMock.Raise(ws => ws.OnCandleReceived += null, candle);

        // Assert
        Assert.NotNull(receivedCandle);
        Assert.Equal(49000, receivedCandle.Open);
    }
}