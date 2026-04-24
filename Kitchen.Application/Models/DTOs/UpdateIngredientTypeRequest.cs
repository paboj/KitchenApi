using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Models.DTOs
{
    public class UpdateIngredientTypeRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitType? Unit { get; set; }
    }
}
