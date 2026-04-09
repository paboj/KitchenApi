namespace Kitchen.Api.Exceptions
{
    public sealed class IncorrectAmountException : KitchenApiException
    {
        public IncorrectAmountException() : base("Incorrect amount.") { }
    }
}
