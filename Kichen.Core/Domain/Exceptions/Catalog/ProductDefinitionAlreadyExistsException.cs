namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class ProductDefinitionAlreadyExistsException : KitchenApiException
    {
        public ProductDefinitionAlreadyExistsException() : base("Product already defined.") { }
    }
}
