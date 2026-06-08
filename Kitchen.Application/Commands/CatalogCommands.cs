using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Commands
{
    public record AddProductDefinitionCommand(
        string Name,
        UnitType Unit,
        Category Category
     );

    public record ModifyProductDefinitionCommand(
        string Name,
        UnitType? Unit,
        Category? Category
     );
}
