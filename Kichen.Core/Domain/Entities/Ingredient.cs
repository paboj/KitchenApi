using System.Text.Json.Serialization;
using System.Xml.Linq;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Domain.Entities
{
    public class Ingredient
    {
        public Guid Id { get; }
        public IngredientName Name { get; set; }
        public double? Amount { get; private set; }
        public StorageLocation? Location { get; private set; }


        public Ingredient(string name, double amount, StorageLocation location) { 
            Id = Guid.NewGuid();
            Name = name;
            Amount = amount;
            Location = location;
        }


        public void AdjustAmount(double? amount)
        {
            if (amount < 0)
            {
                throw new IncorrectAmountException();
            }
            Amount = amount;
        }

        public void PlaceOrMove(StorageLocation? location)
        {
            if (!location.HasValue || location is StorageLocation.Unspecified)
            {
                throw new UnknownLocationException();
            }

            Location = location;
        }
    }
}
