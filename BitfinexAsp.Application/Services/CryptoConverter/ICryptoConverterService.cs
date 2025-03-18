using BitfinexAsp.Services.CryptoConverter.Enum;
using BitfinexAsp.Services.CryptoConverter.Models;

namespace BitfinexAsp.Services.CryptoConverter;

public interface ICryptoConverterService
{
    Task<Portfolio> ConvertPortfolioAsync(Portfolio portfolio);
}