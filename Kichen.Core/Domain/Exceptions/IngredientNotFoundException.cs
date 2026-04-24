namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class IngredientNotFoundException : KitchenApiException
    {
        public IngredientNotFoundException() : base("Ingredient not found.") { }
    }
}
