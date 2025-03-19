using BitfinexAsp.Models.JsonToModelConverter.Converters;

namespace BitfinexAsp.Models.JsonToModelConverter;
using Newtonsoft.Json;

/// <summary>
/// Класс, содержащий настройки сериализации JSON для API-конвертеров.
/// </summary>
public static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        Converters = { new TradeConverter(), new CandleConverter(), new TickerConverter() },
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
}
