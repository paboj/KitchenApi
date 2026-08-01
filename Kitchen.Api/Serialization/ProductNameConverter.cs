using System.Text.Json;
using System.Text.Json.Serialization;
using Kitchen.Core.ValueObjects;

public class ProductNameConverter : JsonConverter<ProductName>
{
    public override ProductName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        return new ProductName(value!);
    }

    public override void Write(Utf8JsonWriter writer, ProductName productName, JsonSerializerOptions options)
    {
        writer.WriteStringValue(productName.Value);
    }
}
