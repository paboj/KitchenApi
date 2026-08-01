namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class UnknownCategoryException : KitchenApiException
    {
        public UnknownCategoryException(string? value = null)
            : base(value is null ? "Unknown category." : $"Unknown category: '{value}'.") { }
    }
}
