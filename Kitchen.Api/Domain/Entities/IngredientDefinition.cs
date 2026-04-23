using System.Text.Json.Serialization;
using Kitchen.Api.Domain.Enums;
using Kitchen.Api.Domain.Exceptions;
using Kitchen.Api.ValueObjects;

namespace Kitchen.Api.Domain.Entities
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
