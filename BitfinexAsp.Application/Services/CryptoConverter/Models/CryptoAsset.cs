using BitfinexAsp.Services.CryptoConverter.Enum;

namespace BitfinexAsp.Services.CryptoConverter.Models;

/// <summary>
/// Класс, представляющий расчет с указанием валюты и количества.
/// </summary>
public class CryptoAsset
{
    public CurrencyType Currency { get; set; }
    public decimal Amount { get; set; }
}