using Kitchen.Api.Domain.Enums;

namespace Kitchen.Api.Models.DTOs
{
    public class IngredientDto
    {
        public string Name { get; set; }
        public double Amount { get; set; }
        public StorageLocation Location { get; set; }
    }
}
