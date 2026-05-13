using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Models.Requests
{
    public class UpdateStockItemRequest
    {
        public double? Amount { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StorageLocation? Location { get; set; }
    }
}
