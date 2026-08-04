using Kitchen.Core.Domain.Enums;

namespace Kitchen.Api.Requests
{
    public record UpdateProductDefinitionRequest(
        UnitType? Unit = null,
        Category? Category = null
    );
}
