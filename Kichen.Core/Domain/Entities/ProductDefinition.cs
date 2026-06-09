using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Domain.Entities
{
    public class ProductDefinition
    {
        //primary key
        public ProductName Name { get; private set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitType Unit { get; private set; } = UnitType.Unspecified;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Category Category { get; private set; } = Category.Unspecified;


        private ProductDefinition() { }
        public ProductDefinition(string name, UnitType unit, Category category) {
            SetName(name);
            ChangeUnitType(unit);
            SetCategory(category);
        }

        public void SetName(string name)
        {
            Name = new ProductName(name);
        }

        public void ChangeUnitType(UnitType? unit)
        {
            if (unit == null) return;

            if (!Enum.IsDefined(typeof(UnitType), unit))
            {
                throw new UnknownUnitTypeException();
            }

            Unit = unit.Value;
        }

        public void SetCategory(Category? category)
        {
            if (category == null) return;

            if (!Enum.IsDefined(typeof(Category), category))
            {
                throw new UnknownCategoryException();
            }

            Category = category.Value;
        }

    }
}
