using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using BitfinexAsp.ApiClients.Bitfinex.REST;
using BitfinexAsp.Models;

namespace BitfinexAsp.ApiClients.Bitfinex.WebSocket;

public class BitfinexWebSocketClient
{
    private readonly ClientWebSocket _webSocket;
    private readonly Uri _uri = new(BitfinexEndpoints.WebSockettradesUrl);
    private int _channelId; // Идентификатор канала

    public event Action<Trade> OnTradeReceived;

    public BitfinexWebSocketClient()
    {
        _webSocket = new ClientWebSocket();
    }

    public async Task ConnectAsync(string pair)
    {
        await _webSocket.ConnectAsync(_uri, CancellationToken.None);
        await SubscribeToTrades( pair);

        _ = ReceiveMessagesAsync();
    }

    private async Task SubscribeToTrades(string symbol)
    {
        var message = new
        {
            @event = "subscribe",
            channel = "trades",
            symbol = symbol
        };

        string jsonMessage = JsonSerializer.Serialize(message);
        await _webSocket.SendAsync(Encoding.UTF8.GetBytes(jsonMessage), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task ReceiveMessagesAsync()
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
        if (json.Contains("subscribed") || json.Contains("hb")) return;

        try
        {
            var jsonArray = JsonSerializer.Deserialize<JsonElement[]>(json);
            if (jsonArray == null || jsonArray.Length < 3) return;

            int channelId = jsonArray[0].GetInt32(); 
            string msgType = jsonArray[1].GetString();

            if (msgType == "te" || msgType == "tu")
            {
                var tradeData = jsonArray[2];

                if (tradeData.ValueKind == JsonValueKind.Array && tradeData.GetArrayLength() >= 4)
                {
                    var trade = new Trade
                    {
                        Id = tradeData[0].ToString(),
                        Time = DateTimeOffset.FromUnixTimeMilliseconds(tradeData[1].GetInt64()),
                        Amount = tradeData[2].GetDecimal(),
                        Price = tradeData[3].GetDecimal(),
                        Pair = "BTCUSD",
                        Side = tradeData[2].GetDecimal() > 0 ? "buy" : "sell"
                    };

                    OnTradeReceived?.Invoke(trade);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка парсинга сообщения: {ex.Message}");
        }
    }

    public async Task DisconnectAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
    }
}
