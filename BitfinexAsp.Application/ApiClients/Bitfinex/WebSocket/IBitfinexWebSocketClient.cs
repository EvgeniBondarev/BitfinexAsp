namespace BitfinexAsp.ApiClients.Bitfinex.WebSocket;

public interface IBitfinexWebSocketClient
{
    /// <summary>
    /// Подключается к WebSocket серверу.
    /// </summary>
    Task ConnectAsync();

    /// <summary>
    /// Получает и обрабатывает входящие сообщения от WebSocket сервера.
    /// </summary>
    Task ReceiveMessagesAsync();

    /// <summary>
    /// Отключается от WebSocket сервера.
    /// </summary>
    Task DisconnectAsync();
}