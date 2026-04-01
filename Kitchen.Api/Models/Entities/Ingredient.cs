namespace Kitchen.Api.Models.Entities
{
    public class Ingredient
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string Unit { get; set; } = string.Empty; // g/ml/szt
    }
}