namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class UnknownUnitTypeException : KitchenApiException
    {
        public UnknownUnitTypeException(string? value = null)
            : base(value is null ? "Unknown unit type." : $"Unknown unit type: '{value}'.") { }
    }
}
