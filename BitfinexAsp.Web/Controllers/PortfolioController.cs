using BitfinexAsp.Services.CryptoConverter;
using BitfinexAsp.Services.CryptoConverter.Enum;
using BitfinexAsp.Services.CryptoConverter.Models;
using BitfinexAsp.Web.ViewModelы;
using Microsoft.AspNetCore.Mvc;

namespace BitfinexAsp.Web.Controllers;

public class PortfolioController : Controller
{
    private readonly ICryptoConverterService _converterService;

    public PortfolioController(ICryptoConverterService converterService)
    {
        _converterService = converterService;
    }

    public async Task<IActionResult> Index()
    {
        var originalPortfolio = new Portfolio
        {
            Assets = new List<CryptoAsset>
            {
                new CryptoAsset { Currency = CurrencyType.BTC, Amount = 1 },
                new CryptoAsset { Currency = CurrencyType.XRP, Amount = 15000 },
                new CryptoAsset { Currency = CurrencyType.XMR, Amount = 50 },
                new CryptoAsset { Currency = CurrencyType.ETC, Amount = 30 }
            }
        };

        var convertedPortfolio = await _converterService.ConvertPortfolioAsync(originalPortfolio);

        var viewModel = new PortfolioViewModel
        {
            OriginalPortfolio = originalPortfolio,
            ConvertedPortfolio = convertedPortfolio
        };

        return View(viewModel);
    }
}