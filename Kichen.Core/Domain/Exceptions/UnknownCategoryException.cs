namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class UnknownCategoryException : KitchenApiException
    {
        public UnknownCategoryException() : base("Uknown category.") { }
    }
}
