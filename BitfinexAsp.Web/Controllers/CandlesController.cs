using BitfinexAsp.ApiClients.Bitfinex.WebSocket;
using BitfinexAsp.ApiClients.Connectors;
using BitfinexAsp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BitfinexAsp.Web.Controllers;

public class CandlesController : Controller
    {
        private static List<Candle> _candles = new();
        private readonly ITestConnector _bitfinexConnector;
        private static string _currentPair = "tBTCUSD";
        private static int _currentPeriod = 60; 
        
        public CandlesController(ITestConnector connector)
        {
            if (_bitfinexConnector == null)
            {
                _bitfinexConnector = connector;
                _bitfinexConnector.CandleSeriesProcessing += OnNewCandleReceived;
                _bitfinexConnector.SubscribeCandles(_currentPair, _currentPeriod, 15);
            }
        }

        private void OnNewCandleReceived(Candle candle)
        {
            _candles.Insert(0, candle);
            if (_candles.Count > 15) _candles.RemoveAt(_candles.Count - 1);
        }

        public async Task<IActionResult> Index(string pair, int? periodInSec, long? count, DateTimeOffset? from, DateTimeOffset? to)
        {
            if (!string.IsNullOrEmpty(pair) && pair != _currentPair)
            {
                _bitfinexConnector.UnsubscribeCandles(_currentPair);
                _bitfinexConnector.SubscribeCandles(pair, periodInSec ?? _currentPeriod, count, from, to);
                _currentPair = pair;
                _currentPeriod = periodInSec ?? _currentPeriod;
                _candles.Clear();
            }

            ViewBag.Pair = pair;
            ViewBag.PeriodInSec = periodInSec;
            ViewBag.Count = count;
            ViewBag.From = from;
            ViewBag.To = to;

            var filteredCandles = await _bitfinexConnector.GetCandleSeriesAsync(pair ?? _currentPair, periodInSec ?? _currentPeriod, from, count, to);

            return View(filteredCandles.ToList());
        }

        public async Task<IActionResult> GetCandles(string pair, int? periodInSec, long? count, DateTimeOffset? from, DateTimeOffset? to)
        {
            if (!string.IsNullOrEmpty(pair) && pair != _currentPair)
            {
                _bitfinexConnector.UnsubscribeCandles(_currentPair);
                _bitfinexConnector.SubscribeCandles(pair, periodInSec ?? _currentPeriod, count, from, to);
                _currentPair = pair;
                _candles.Clear();
            }

            var filteredCandles = await _bitfinexConnector.GetCandleSeriesAsync(pair ?? _currentPair, periodInSec ?? _currentPeriod, from, count, to);

            return PartialView("_CandlesTable", filteredCandles.ToList());
        }
    }