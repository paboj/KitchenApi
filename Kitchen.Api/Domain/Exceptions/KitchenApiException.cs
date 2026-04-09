namespace Kitchen.Api.Domain.Exceptions
{
    public abstract class KitchenApiException : Exception
    {
        protected KitchenApiException(string message) : base(message) {}
    }
}
