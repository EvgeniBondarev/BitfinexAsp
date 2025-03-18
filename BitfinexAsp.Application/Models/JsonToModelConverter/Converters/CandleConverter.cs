using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitfinexAsp.Models.JsonToModelConverter.Converters;

public class CandleConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<Candle>);
    }

    public override List<Candle> ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var candles = new List<Candle>();
        var array = JArray.Load(reader);

        foreach (var item in array)
        {
            var candle = new Candle
            {
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(item[0].Value<long>()),
                OpenPrice = item[1].Value<decimal>(),
                ClosePrice = item[2].Value<decimal>(),
                HighPrice = item[3].Value<decimal>(),
                LowPrice = item[4].Value<decimal>(),
                TotalVolume = item[5].Value<decimal>()
            };
            candles.Add(candle);
        }

        return candles;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
