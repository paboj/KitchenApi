using System.Text.Json;
using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;

public class CategoryConverter : JsonConverter<Category>
{
    public override Category Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var raw = reader.GetString();
        var value = raw?.ToLower();

        return value switch
        {
            "" or "-" or "unspecified" => Category.Unspecified,
            "meat" or "mięso" => Category.Meat,
            "vegetables" or "warzywa" => Category.Vegetables,
            "dairy" or "nabiał" => Category.Dairy,
            "drygoods" or "sypkie" => Category.DryGoods,
            "spices" or "przyprawy" => Category.Spices,
            "other" or "inne" => Category.Other,
            _ => throw new UnknownCategoryException(raw)
        };
    }

    public override void Write(Utf8JsonWriter writer, Category category, JsonSerializerOptions options)
    {
        writer.WriteStringValue(category.ToDescription());
    }
}
