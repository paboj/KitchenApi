using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Commands
{
    public record AddToStockCommand(
        string Name,
        double Amount,
        StorageLocation Location
     );

    public record ModifyInStockCommand(
        string Name,
        double? Amount,
        StorageLocation? Location
     );

}
