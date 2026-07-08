using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Models.Requests
{
    public class UpdateProductDefinitionRequest
    {
        public UnitType? Unit { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Category? Category { get; set; }
    }
}
