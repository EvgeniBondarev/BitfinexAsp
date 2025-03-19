namespace BitfinexAsp.Services.CryptoConverter.Enum;

/// <summary>
/// Перечисление, представляющее доступные типы валют на бирже Bitfinex.
/// </summary>
public enum CurrencyType
{
    BTC,
    XRP,
    XMR,
    ETC, //В задании было DASH, заменил т.к не нашел в https://api-pub.bitfinex.com/v2/tickers?symbols=ALL
    USDT
}