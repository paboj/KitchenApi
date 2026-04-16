using System.Text.Json.Serialization;
using Kitchen.Api.Domain.Enums;

namespace Kitchen.Api.Models.DTOs
{
    public class CreateIngredientDefinitionRequest
    {
        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitType Unit { get; set; }
    }
}
