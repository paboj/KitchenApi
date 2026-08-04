using Kitchen.Core.Domain.Enums;

namespace Kitchen.Api.Requests
{
    public record UpdateStockItemRequest(
        string? Name = null,
        double? Amount = null,
        StorageLocation? Location = null,
        DateOnly? ExpirationDate = null
    );
}
