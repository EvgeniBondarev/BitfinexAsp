using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitfinexAsp.Models.JsonToModelConverter.Converters;

public class TradeConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<Trade>);
    }

    public override List<Trade> ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var trades = new List<Trade>();
        var array = JArray.Load(reader);

        foreach (var item in array)
        {
            var trade = new Trade
            {
                Id = item[0].ToString(),
                Time = DateTimeOffset.FromUnixTimeMilliseconds(item[1].Value<long>()),
                Amount = item[2].Value<decimal>(),
                Price = item[3].Value<decimal>(),
                Side = item[2].Value<decimal>() > 0 ? "buy" : "sell" 
            };
            trades.Add(trade);
        }

        return trades;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}