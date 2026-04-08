using System.Text.Json.Serialization;
using Kitchen.Api.Models.Enums;

namespace Kitchen.Api.Models.Entities
{
    public class Ingredient
    {
        public Guid Id { get; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; private set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitType Unit { get; private set; }

        public Ingredient(string name, double amount, UnitType unit) { 
            Id = Guid.NewGuid();
            Name = name;
            Amount = amount;
            Unit = unit;
        }

        public void SetAmount(double amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException (nameof(amount));
            }
            Amount = amount;
        }

        public void ChangeUnitType(UnitType unit)
        {
            if (unit < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unit));
            }
            Unit = unit;
        }
    }
}
