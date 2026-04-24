using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Models.DTOs
{
    public class CreateIngredientTypeRequest
    {
        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitType Unit { get; set; }
    }
}
