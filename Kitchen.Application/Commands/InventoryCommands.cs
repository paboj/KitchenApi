using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Commands
{
    public record AddStockItemCommand(
        string Name,
        double Amount,
        StorageLocation Location
     );

    public record ModifyStockItemCommand(
        Guid Id,
        string? Name,
        double? Amount,
        StorageLocation? Location
     );

}
