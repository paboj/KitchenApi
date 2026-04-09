using System.Text.Json.Serialization;
using Kitchen.Api.Exceptions;
using Kitchen.Api.Models.Enums;

namespace Kitchen.Api.Models.Entities
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
