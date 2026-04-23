namespace Kitchen.Api.Domain.Exceptions
{
    public sealed class IngredientNotFoundException : KitchenApiException
    {
        public IngredientNotFoundException() : base("Ingredient not found.") { }
    }
}
