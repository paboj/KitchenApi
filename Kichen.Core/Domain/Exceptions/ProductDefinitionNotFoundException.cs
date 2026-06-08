namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class ProductDefinitionNotFoundException : KitchenApiException
    {
        public ProductDefinitionNotFoundException() : base("Item not found.") { }
    }
}
