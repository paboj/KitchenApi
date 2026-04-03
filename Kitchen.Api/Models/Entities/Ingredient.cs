using System.Text.Json.Serialization;
using Kitchen.Api.Models.Enums;

namespace Kitchen.Api.Models.Entities
{
    public class Ingredient
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitType Unit { get; set; }
    }
}
