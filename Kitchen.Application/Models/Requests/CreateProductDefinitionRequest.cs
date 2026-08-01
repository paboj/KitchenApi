using Kitchen.Core.Domain.Enums;

namespace Kitchen.Application.Models.Requests
{
    public class CreateProductDefinitionRequest
    {
        public string Name { get; set; }
        public UnitType Unit { get; set; }
        public Category Category { get; set; }
    }
}
