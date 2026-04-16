using Kitchen.Api.Domain.Enums;

namespace Kitchen.Api.Commands
{
    public record AddToCatalogCommand(
        string Name,
        UnitType Unit
     );

    public record ModifyInCatalogCommand(
        string Name,
        UnitType? Unit
     );
}
