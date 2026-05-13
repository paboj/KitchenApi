namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class InvalidProductNameException : KitchenApiException
    {
        public InvalidProductNameException() : base("Invalid name.") { }
    }
}
