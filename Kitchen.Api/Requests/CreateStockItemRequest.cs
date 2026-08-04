using Kitchen.Core.Domain.Enums;

namespace Kitchen.Api.Requests
{
    public record CreateStockItemRequest(
        string Name,
        double Amount = 0,
        StorageLocation Location = StorageLocation.Unspecified,
        DateOnly? ExpirationDate = null
    );
}
