using BitfinexAsp.ApiClients.Connectors;
using BitfinexAsp.ApiClients.Connectors.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace BitfinexAsp.Web.Controllers;

public class BitfinexController : Controller
{
    private readonly ITestConnector _bitfinexConnector;

    public BitfinexController(ITestConnector bitfinexConnector)
    {
        _bitfinexConnector = bitfinexConnector;
    }
    
    public async Task<IActionResult> Trades(string pair = "tBTCUSD", int maxCount = 10)
    {
        ViewBag.Pair = pair;
        ViewBag.MaxCount = maxCount;
        var trades = await _bitfinexConnector.GetNewTradesAsync(pair, maxCount);
        return View(trades);
    }

    public async Task<IActionResult> Candles(string pair = "tBTCUSD", int periodInSec = 60, long? count = 10, DateTimeOffset? from = null, DateTimeOffset? to = null)
    {
        ViewBag.Pair = pair;
        ViewBag.PeriodInSec = periodInSec;
        ViewBag.Count = count;
        ViewBag.From = from;
        ViewBag.To = to;
        var candles = await _bitfinexConnector.GetCandleSeriesAsync(pair, periodInSec, from, count, to);
        return View(candles);
    }

    public async Task<IActionResult> Ticker(string symbol = "tBTCUSD")
    {
        ViewBag.Symbol = symbol;
        if (_bitfinexConnector is BitfinexConnector bitfinexConnector)
        {
            var ticker = await bitfinexConnector.GetTickerAsync(symbol);
            return View(ticker);
        }
        throw new InvalidOperationException("Коннектор не поддерживает метод GetTickerAsync.");
    }

}