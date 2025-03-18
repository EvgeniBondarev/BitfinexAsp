namespace BitfinexAsp.ApiClients.Bitfinex.WebSocket;

using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BitfinexWebSocketClient : IDisposable
{
    private const string WebSocketUrl = "wss://api-pub.bitfinex.com/ws/2";
    private ClientWebSocket _webSocket;
    private readonly Dictionary<int, string> _channelIdToPair = new Dictionary<int, string>();
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public event Action<Trade> NewBuyTrade;
    public event Action<Trade> NewSellTrade;

    public async Task ConnectAsync()
    {
        _webSocket = new ClientWebSocket();
        await _webSocket.ConnectAsync(new Uri(WebSocketUrl), _cancellationTokenSource.Token);
        Console.WriteLine("WebSocket connected.");
        _ = ReceiveMessagesAsync();
    }

    public async Task SubscribeTradesAsync(string pair, int maxCount = 100)
    {
        var subscribeMessage = new
        {
            @event = "subscribe",
            channel = "trades",
            symbol = pair
        };

        string jsonMessage = JsonConvert.SerializeObject(subscribeMessage);
        byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
        await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        Console.WriteLine($"Subscribed to trades for {pair}.");
    }

    public async Task UnsubscribeTradesAsync(string pair)
    {
        var unsubscribeMessage = new
        {
            @event = "unsubscribe",
            channel = "trades",
            symbol = pair
        };

        string jsonMessage = JsonConvert.SerializeObject(unsubscribeMessage);
        byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
        await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        Console.WriteLine($"Unsubscribed from trades for {pair}.");
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024 * 4];
        while (_webSocket.State == WebSocketState.Open)
        {
            try
            {
                WebSocketReceiveResult result;
                var message = new List<byte>();

                do
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                    message.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                string messageString = Encoding.UTF8.GetString(message.ToArray());
                Console.WriteLine($"Received message: {messageString}");
                ProcessMessage(messageString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
                break;
            }
        }
    }

    private void ProcessMessage(string message)
    {
        try
        {
            var json = JToken.Parse(message);

            // Обработка подписки
            if (json["event"]?.ToString() == "subscribed")
            {
                int channelId = json["chanId"].ToObject<int>();
                string pair = json["symbol"].ToString();
                _channelIdToPair[channelId] = pair;
                Console.WriteLine($"Subscribed to {pair} with channel ID {channelId}");
                return;
            }

            // Обработка данных о сделках
            if (json is JArray array && array.Count >= 2)
            {
                int channelId = array[0].ToObject<int>();
                if (_channelIdToPair.TryGetValue(channelId, out string pair))
                {
                    if (array[1] is JArray snapshotOrUpdate)
                    {
                        if (snapshotOrUpdate[0] is JArray) // Это снапшот
                        {
                            foreach (var tradeArray in snapshotOrUpdate)
                            {
                                ProcessTrade(tradeArray.ToObject<float[]>(), pair);
                            }
                        }
                        else if (snapshotOrUpdate[1].ToString() == "te" || snapshotOrUpdate[1].ToString() == "tu") // Это обновление
                        {
                            ProcessTrade(snapshotOrUpdate[2].ToObject<float[]>(), pair);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex.Message}");
        }
    }

    private void ProcessTrade(float[] tradeData, string pair)
    {
        try
        {
            var trade = new Trade
            {
                Id = (long)tradeData[0],
                Time = DateTimeOffset.FromUnixTimeMilliseconds((long)tradeData[1]),
                Amount = tradeData[2],
                Price = tradeData[3],
                Pair = pair
            };

            if (trade.Amount > 0)
            {
                NewBuyTrade?.Invoke(trade);
            }
            else
            {
                NewSellTrade?.Invoke(trade);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing trade: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _webSocket?.Dispose();
        _cancellationTokenSource.Cancel();
    }
}
public class Trade
{
    public long Id { get; set; }
    public DateTimeOffset Time { get; set; }
    public float Amount { get; set; }
    public float Price { get; set; }
    public string Pair { get; set; }
}