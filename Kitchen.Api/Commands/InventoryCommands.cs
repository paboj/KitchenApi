using Kitchen.Api.Domain.Enums;

namespace Kitchen.Api.Commands
{
    public record AddToStockCommand(
        string Name,
        double Amount,
        StorageLocation Location
     );

    public record ModifyIngredientCommand(
        string Name,
        double? Amount,
        StorageLocation? Location
     );

}
