using Kitchen.Api.Models.Enums;

namespace Kitchen.Api.Models.DTOs
{
    public class CreateIngredientRequest
    {
        public string Name { get; set; }
        public double Amount { get; set; }
        public UnitType Unit { get; set; }
    }
}
