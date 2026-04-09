using System.Text.Json.Serialization;
using Kitchen.Api.Domain.Enums;
using Kitchen.Api.Domain.Exceptions;

namespace Kitchen.Api.Domain.Entities
{
    public class IngredientDefinition
    {
        //primary key
        public string Name { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitType Unit { get; private set; }

        public IngredientDefinition(string name, UnitType unit) { 
            Name = name;
            Unit = unit;
        }

        public void ChangeUnitType(UnitType unit)
        {
            if (!Enum.IsDefined(unit) || unit is UnitType.Unspecified)
            {
                throw new UnknownUnitTypeException();
            }

            Unit = unit;
        }
    }
}
