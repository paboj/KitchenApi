using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Models.DTOs
{
    public class CreateIngredientRequest
    {
        public string Name { get; set; }
        public double Amount { get; set; } = 0;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StorageLocation Location { get; set; } = StorageLocation.Unspecified;
    }
}
