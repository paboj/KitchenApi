using System.Text.Json.Serialization;
using Kitchen.Api.Domain.Enums;

namespace Kitchen.Api.Models.DTOs
{
    public class UpdateIngredientRequest
    {
        public double? Amount { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StorageLocation? Location { get; set; }
    }
}
