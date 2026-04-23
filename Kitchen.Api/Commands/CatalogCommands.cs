using Kitchen.Api.Domain.Enums;

namespace Kitchen.Api.Commands
{
    public record AddTypeCatalogCommand(
        string Name,
        UnitType Unit
     );

    public record ModifyTypeCatalogCommand(
        string Name,
        UnitType? Unit
     );
}
