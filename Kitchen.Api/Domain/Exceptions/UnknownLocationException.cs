namespace Kitchen.Api.Domain.Exceptions
{
    public class UnknownLocationException : KitchenApiException
    {
        public UnknownLocationException() : base("Uknown location. Available: fridge, freezer, pantry.") { }
    }
}
