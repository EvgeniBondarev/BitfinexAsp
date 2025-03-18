using BitfinexAsp.Services.CryptoConverter.Enum;

namespace BitfinexAsp.Services.CryptoConverter.Models;

public class CryptoAsset
{
    public CurrencyType Currency { get; set; }
    public decimal Amount { get; set; }
}