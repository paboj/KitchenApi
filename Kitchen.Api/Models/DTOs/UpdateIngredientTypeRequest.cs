using System.Text.Json.Serialization;
using Kitchen.Api.Domain.Enums;

namespace Kitchen.Api.Models.DTOs
{
    public class UpdateIngredientTypeRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitType? Unit { get; set; }
    }
}
