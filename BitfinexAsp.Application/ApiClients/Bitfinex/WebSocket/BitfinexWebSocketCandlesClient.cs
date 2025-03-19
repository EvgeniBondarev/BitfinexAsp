using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using BitfinexAsp.ApiClients.Bitfinex.REST;
using BitfinexAsp.Models;
using BitfinexAsp.Utils;

namespace BitfinexAsp.ApiClients.Bitfinex.WebSocket;

public class BitfinexWebSocketCandlesClient : IBitfinexWebSocketClient
{
    private readonly ClientWebSocket _webSocket;
    private readonly Uri _uri = new(BitfinexEndpoints.WebSocketUrl);
    private readonly Dictionary<int, string> _channels = new(); 
    private int _channelId;

    public event Action<Candle>? OnCandleReceived;

    public BitfinexWebSocketCandlesClient()
    {
        _webSocket = new ClientWebSocket();
    }

    public async Task ConnectAsync()
    {
        await _webSocket.ConnectAsync(_uri, CancellationToken.None);
        _ = ReceiveMessagesAsync();
    }

    public async Task SubscribeCandles(string pair, int periodInSec)
    {
        string key = $"trade:{TimeConvertor.ConvertSecondsToMinutes(periodInSec)}m:{pair}";
        var message = new
        {
            @event = "subscribe",
            channel = "candles",
            key = key
        };

        string jsonMessage = JsonSerializer.Serialize(message);
        await _webSocket.SendAsync(Encoding.UTF8.GetBytes(jsonMessage), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task UnsubscribeCandles(string pair, int periodInSec)
    {
        string key = $"trade:{TimeConvertor.ConvertSecondsToMinutes(periodInSec)}m:{pair}";

        var channelId = _channels.FirstOrDefault(x => x.Value == key).Key;
        if (channelId == 0) return;

        var message = new
        {
            @event = "unsubscribe",
            chanId = channelId
        };

        string jsonMessage = JsonSerializer.Serialize(message);
        await _webSocket.SendAsync(Encoding.UTF8.GetBytes(jsonMessage), WebSocketMessageType.Text, true, CancellationToken.None);
        _channels.Remove(channelId);
    }

    public async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[8192];

        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                break;
            }

            string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            ParseMessage(json);
        }
    }

    private void ParseMessage(string json)
    {
        if (json.Contains("subscribed"))
        {
            var response = JsonSerializer.Deserialize<JsonDocument>(json);
            if (response != null && response.RootElement.TryGetProperty("chanId", out var chanId) &&
                response.RootElement.TryGetProperty("key", out var key))
            {
                _channels[chanId.GetInt32()] = key.GetString();
            }
            return;
        }

        if (json.Contains("hb")) return; // Heartbeat игнорируем

        try
        {
            var jsonArray = JsonSerializer.Deserialize<JsonElement[]>(json);
            if (jsonArray == null || jsonArray.Length < 2) return;

            int channelId = jsonArray[0].GetInt32();
            var data = jsonArray[1];

            if (data.ValueKind == JsonValueKind.Array)
            {
                var candleData = data.EnumerateArray().ToArray();
                if (candleData.Length >= 6)
                {
                    var candle = new Candle
                    {
                        OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(candleData[0].GetInt64()),
                        OpenPrice = candleData[1].GetDecimal(),
                        ClosePrice = candleData[2].GetDecimal(),
                        HighPrice = candleData[3].GetDecimal(),
                        LowPrice = candleData[4].GetDecimal(),
                        TotalVolume = candleData[5].GetDecimal(),
                        Pair = _channels.ContainsKey(channelId) ? _channels[channelId].Split(':')[2] : "Unknown"
                    };

                    OnCandleReceived?.Invoke(candle);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка парсинга свечи: {ex.Message}");
        }
    }

    public async Task DisconnectAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
    }
}
