using System.ComponentModel.DataAnnotations;
using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Models.Requests
{
    public class CreateStockItemRequest
    {
        public string Name { get; set; }
        public double Amount { get; set; } = 0;
        public StorageLocation Location { get; set; } = StorageLocation.Unspecified;
        public DateOnly? ExpirationDate { get; set; }
    }
}
