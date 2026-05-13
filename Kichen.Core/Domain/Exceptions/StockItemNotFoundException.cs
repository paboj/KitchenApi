namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class StockItemNotFoundException : KitchenApiException
    {
        public StockItemNotFoundException() : base("Item not found.") { }
    }
}
