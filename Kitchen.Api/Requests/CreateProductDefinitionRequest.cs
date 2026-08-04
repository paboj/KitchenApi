using Kitchen.Core.Domain.Enums;

namespace Kitchen.Api.Requests
{
    public record CreateProductDefinitionRequest(
        string Name,
        UnitType Unit,
        Category Category
    );
}
