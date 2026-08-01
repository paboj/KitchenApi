using System.Text.Json;
using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;

public class StorageLocationConverter : JsonConverter<StorageLocation>
{
    public override StorageLocation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var raw = reader.GetString();
        var value = raw?.ToLower();

        return value switch
        {
            "" or "-" or "unspecified" => StorageLocation.Unspecified,
            "fridge" or "lodówka" => StorageLocation.Fridge,
            "freezer" or "zamrażarka" => StorageLocation.Freezer,
            "pantry" or "szafki" => StorageLocation.Pantry,
            _ => throw new UnknownLocationException(raw)
        };
    }

    public override void Write(Utf8JsonWriter writer, StorageLocation storageLocation, JsonSerializerOptions options)
    {
        writer.WriteStringValue(storageLocation.ToDescription());
    }
}
