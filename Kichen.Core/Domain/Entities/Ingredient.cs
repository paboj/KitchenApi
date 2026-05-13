using System.Text.Json.Serialization;
using System.Xml.Linq;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Domain.Entities
{
    public class Ingredient
    {
        public IngredientId Id { get; private set; }
        public IngredientName Name { get; set; }
        public double Amount { get; private set; } = 0;
        public StorageLocation Location { get; private set; } = StorageLocation.Unspecified;
        public IngredientName? TypeName { get; private set; }
        public IngredientType? Type { get; set; }

        private Ingredient() { }

        public Ingredient(string name, double amount, StorageLocation location, IngredientType? type) { 
            Id = new IngredientId(Guid.NewGuid());
            Name = name;
            AdjustAmount(amount);
            PlaceOrMove(location);
            AssignType(type);
        }

        public void AssignType(IngredientType? type)
        {
            if (type != null)
            {
                Type = type;
                TypeName = type.Name;
            }
        }


        public void AdjustAmount(double? amount)
        {
            if (amount is null) return;

            if (amount < 0)
            {
                throw new IncorrectAmountException();
            }
            Amount = amount.Value;
        }

        public void PlaceOrMove(StorageLocation? location)
        {
            if (location is null) return;

            if (!Enum.IsDefined(typeof(StorageLocation), location))
            {
                throw new UnknownLocationException();
            }

            Location = location.Value;
        }
    }
}
