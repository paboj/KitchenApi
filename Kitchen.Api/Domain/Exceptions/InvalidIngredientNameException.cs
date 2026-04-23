namespace Kitchen.Api.Domain.Exceptions
{
    public sealed class InvalidIngredientNameException : KitchenApiException
    {
        public InvalidIngredientNameException() : base("Name cannot be empty.") { }
    }
}
