using Kitchen.Api.Domain.Enums;

namespace Kitchen.Api.Commands
{
    public record AddToStockCommand(string Name, double Amount, StorageLocation Location);
    public record CreateDefinitionCommand(string Name, UnitType Unit);
    public record UpdateQuantityCommand(Guid Id, double Amount);
}
