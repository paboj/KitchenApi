using Kitchen.Core.Domain.Enums;

namespace Kitchen.Api.Models.Requests
{
    public class UpdateProductDefinitionRequest
    {
        public UnitType? Unit { get; set; }
        public Category? Category { get; set; }
    }
}
