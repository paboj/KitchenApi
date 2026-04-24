using System.Text.Json.Serialization;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Domain.Entities
{
    public class IngredientType
    {
        //primary key
        public IngredientName Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitType? Unit { get; private set; }

        public IngredientType(string name, UnitType unit) { 
            Name = name;
            Unit = unit;
        }

        public void ChangeUnitType(UnitType? unit)
        {
            if (!unit.HasValue || unit is UnitType.Unspecified)
            {
                throw new UnknownUnitTypeException();
            }

            Unit = unit;
        }
    }
}
