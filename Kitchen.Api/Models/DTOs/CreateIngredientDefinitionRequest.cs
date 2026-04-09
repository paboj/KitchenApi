using Kitchen.Api.Models.Enums;

namespace Kitchen.Api.Models.DTOs
{
    public class CreateIngredientDefinitionRequest
    {
        public string Name { get; set; }
        public UnitType Unit { get; set; }
    }
}
