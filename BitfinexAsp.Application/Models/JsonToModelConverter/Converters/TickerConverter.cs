using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitfinexAsp.Models.JsonToModelConverter.Converters;

public class TickerConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Ticker);
    }

    public override Ticker ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var array = JArray.Load(reader);

        var ticker = new Ticker
        {
            Bid = array[0].Value<float>(),
            BidSize = array[1].Value<float>(),
            Ask = array[2].Value<float>(),
            AskSize = array[3].Value<float>(),
            DailyChange = array[4].Value<float>(),
            DailyChangeRelative = array[5].Value<float>(),
            LastPrice = array[6].Value<float>(),
            Volume = array[7].Value<float>(),
            High = array[8].Value<float>(),
            Low = array[9].Value<float>()
        };

        return ticker;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}