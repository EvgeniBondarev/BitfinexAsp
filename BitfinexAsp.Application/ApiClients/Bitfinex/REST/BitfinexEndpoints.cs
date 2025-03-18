namespace BitfinexAsp.ApiClients.Bitfinex.REST;

public static class BitfinexEndpoints
{
    public const string BaseUrl = "https://api-pub.bitfinex.com/v2";

    public static string GetTrades(string symbol, int limit)
    {
        string queryString = BuildQueryString(("limit", limit));
        return $"{BaseUrl}/trades/{symbol}/hist{queryString}";
    }

    public static string GetCandles(string candle, string section, long? start, long? end, long? limit)
    {
        string queryString = BuildQueryString(("start", start), ("end", end), ("limit", limit));
        return $"{BaseUrl}/candles/{candle}/{section}{queryString}";
    }

    private static string BuildQueryString(params (string Key, object? Value)[] parameters)
    {
        var queryParams = parameters
            .Where(param => param.Value != null)
            .Select(param => $"{param.Key}={param.Value}")
            .ToList();
    
        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }
    public static string GetTicker(string symbol)
    {
        return $"{BaseUrl}/ticker/{symbol}";
    }
}
