namespace Kitchen.Api.Exceptions
{
    public abstract class KitchenApiException : Exception
    {
        protected KitchenApiException(string message) : base(message) {}
    }
}
