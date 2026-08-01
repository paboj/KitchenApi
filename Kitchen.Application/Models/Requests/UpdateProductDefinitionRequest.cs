using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Models.Requests
{
    public class UpdateProductDefinitionRequest
    {
        public UnitType? Unit { get; set; }
        public Category? Category { get; set; }
    }
}
