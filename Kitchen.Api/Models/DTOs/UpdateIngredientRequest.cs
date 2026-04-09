using Kitchen.Api.Models.Enums;

namespace Kitchen.Api.Models.DTOs
{
    public class UpdateIngredientRequest
    {
        public double Amount { get; set; }
        public StorageLocation Location { get; set; }
    }
}
