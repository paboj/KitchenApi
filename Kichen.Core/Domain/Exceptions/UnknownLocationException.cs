namespace Kitchen.Core.Domain.Exceptions
{
    public class UnknownLocationException : KitchenApiException
    {
        public UnknownLocationException() : base("Unknown location. Available: fridge, freezer, pantry.") { }
    }
}
