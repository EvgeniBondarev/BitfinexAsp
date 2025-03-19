namespace BitfinexAsp.Services.CryptoConverter.Models;

/// <summary>
/// Класс, представляющий портфель криптоактивов.
/// </summary>
public class Portfolio
{
    public List<CryptoAsset> Assets { get; set; } = new();
}