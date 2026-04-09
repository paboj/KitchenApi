using Kitchen.Api.Domain.Enums;

namespace Kitchen.Api.Models.DTOs
{
    public class IngredientDefinitionDto
    {
        public string Name { get; set; }
        public UnitType Unit { get; set; }
    }
}
