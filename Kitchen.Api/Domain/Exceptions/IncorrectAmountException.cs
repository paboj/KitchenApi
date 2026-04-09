namespace Kitchen.Api.Domain.Exceptions
{
    public sealed class IncorrectAmountException : KitchenApiException
    {
        public IncorrectAmountException() : base("Incorrect amount.") { }
    }
}
