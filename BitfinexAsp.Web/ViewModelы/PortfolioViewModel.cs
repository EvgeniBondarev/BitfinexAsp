using BitfinexAsp.Services.CryptoConverter.Models;

namespace BitfinexAsp.Web.ViewModelы;

public class PortfolioViewModel
{
    public Portfolio OriginalPortfolio { get; set; }
    public Portfolio ConvertedPortfolio { get; set; }
}
