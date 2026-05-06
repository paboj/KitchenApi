using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Commands
{
    public record AddTypeCatalogCommand(
        string Name,
        UnitType Unit,
        Category Category
     );

    public record ModifyTypeCatalogCommand(
        string Name,
        UnitType? Unit,
        Category? Category
     );
}
