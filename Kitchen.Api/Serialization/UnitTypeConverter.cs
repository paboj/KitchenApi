using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;

public class UnitTypeConverter : JsonConverter<UnitType>
{
    public override UnitType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()?.ToLower();

        return value switch
        {
            "szt" or "sztuk" or "pieces" => UnitType.Pieces,
            "kg" or "kilograms" => UnitType.Kilograms,
            "l" or "liters" or "litry" => UnitType.Liters,
            _ => UnitType.Unspecified
        };
    }

    public override void Write(Utf8JsonWriter writer, UnitType value, JsonSerializerOptions options)
    {
        // Tutaj decydujesz co API wysyła na zewnątrz (np. zawsze opis "szt")
        writer.WriteStringValue(GetDescription(value));
    }

    private string GetDescription(UnitType value)
    {
        return value.GetType()
            .GetField(value.ToString())
            ?.GetCustomAttribute<DescriptionAttribute>()
            ?.Description ?? value.ToString().ToLower();
    }
}