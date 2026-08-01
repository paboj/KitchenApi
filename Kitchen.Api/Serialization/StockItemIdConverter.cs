using System.Text.Json;
using System.Text.Json.Serialization;
using Kitchen.Core.ValueObjects;

public class StockItemIdConverter : JsonConverter<StockItemId>
{
    public override StockItemId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetGuid();

        return new StockItemId(value);
    }

    public override void Write(Utf8JsonWriter writer, StockItemId stockItemId, JsonSerializerOptions options)
    {
        writer.WriteStringValue(stockItemId.Value);
    }
}
