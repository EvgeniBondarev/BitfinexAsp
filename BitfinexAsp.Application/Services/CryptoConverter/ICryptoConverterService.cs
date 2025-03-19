using BitfinexAsp.Services.CryptoConverter.Enum;
using BitfinexAsp.Services.CryptoConverter.Models;

namespace BitfinexAsp.Services.CryptoConverter;

public interface ICryptoConverterService
{
    /// <summary>
    /// Конвертирует все активы в портфеле в эквивалентные суммы в каждой из доступных валют.
    /// </summary>
    /// <param name="portfolio">Исходный криптовалютный портфель.</param>
    /// <returns>Преобразованный портфель с пересчитанными значениями.</returns>
    Task<Portfolio> ConvertPortfolioAsync(Portfolio portfolio);
}