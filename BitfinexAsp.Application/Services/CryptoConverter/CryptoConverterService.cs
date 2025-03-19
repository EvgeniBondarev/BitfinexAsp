
using BitfinexAsp.ApiClients.Bitfinex.REST;
using BitfinexAsp.Services.CryptoConverter.Enum;
using BitfinexAsp.Services.CryptoConverter.Models;

namespace BitfinexAsp.Services.CryptoConverter;

/// <summary>
/// Сервис для конвертации криптовалютного портфеля между разными валютами.
/// </summary>
public class CryptoConverterService : ICryptoConverterService
{
    private readonly BitfinexClient _bitfinexClient;
    private readonly List<CurrencyType> _currencies;

    public CryptoConverterService(BitfinexClient bitfinexClient)
    {
        _bitfinexClient = bitfinexClient;
        _currencies = [ CurrencyType.BTC, CurrencyType.XRP, CurrencyType.XMR, CurrencyType.ETC, CurrencyType.USDT ];
    }

    public async Task<Portfolio> ConvertPortfolioAsync(Portfolio portfolio)
    {
        var conversionRates = new Dictionary<CurrencyType, decimal>();
        
        // Курсы для конвертации
        foreach (var currency in _currencies)
        {
            if (currency != CurrencyType.USDT) 
            {
                string symbol = $"t{currency}USD";
                var ticker = await _bitfinexClient.GetTickersAsync(symbol);
                conversionRates[currency] = (decimal)ticker.LastPrice;
            }
            else
            {
                conversionRates[currency] = 1m;
            }
        }
        
        var convertedPortfolio = new Portfolio();
    
        // Баланс по каждой криптовалюте с переводом в целевую 
        foreach (CurrencyType targetCurrency in _currencies)
        {
            decimal totalInTargetCurrency = 0m;

            foreach (var asset in portfolio.Assets)
            {
                decimal amountInUsdt = asset.Amount * conversionRates[asset.Currency];
                decimal amountInTargetCurrency = amountInUsdt / conversionRates[targetCurrency]; 
                totalInTargetCurrency += amountInTargetCurrency;
            }
            convertedPortfolio.Assets.Add(new CryptoAsset
            {
                Currency = targetCurrency,
                Amount = totalInTargetCurrency
            });
        }
        return convertedPortfolio;
    }
}