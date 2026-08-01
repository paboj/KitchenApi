using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Models.Requests
{
    public class UpdateStockItemRequest
    {
        public string? Name { get; set; }
        public double? Amount { get; set; }
        public StorageLocation? Location { get; set; }
        public DateOnly? ExpirationDate { get; set; }
    }
}
