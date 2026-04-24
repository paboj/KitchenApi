namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class UnknownUnitTypeException : KitchenApiException
    {
        public UnknownUnitTypeException() : base("Uknown unit type. Available: pieces, grams, mililiters.") { }
    }
}
