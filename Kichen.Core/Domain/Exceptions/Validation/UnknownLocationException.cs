namespace Kitchen.Core.Domain.Exceptions
{
    public class UnknownLocationException : KitchenApiException
    {
        public UnknownLocationException(string? value = null)
            : base(value is null ? "Unknown location." : $"Unknown location: '{value}'.") { }
    }
}
