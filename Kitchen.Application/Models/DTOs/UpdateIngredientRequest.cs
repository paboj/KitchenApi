using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Models.DTOs
{
    public class UpdateIngredientRequest
    {
        public double? Amount { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StorageLocation? Location { get; set; }
    }
}
