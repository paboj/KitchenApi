using System.Text.Json.Serialization;
using Kitchen.Api.Domain.Enums;
using Kitchen.Api.Domain.Exceptions;

namespace Kitchen.Api.Domain.Entities
{
    public class Ingredient
    {
        public Guid Id { get; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; private set; }
        public StorageLocation Location { get; private set; }


        public Ingredient(string name, double amount, StorageLocation location) { 
            Id = Guid.NewGuid();
            Name = name;
            Amount = amount;
            Location = location;
        }

        public void AdjustAmount(double amount)
        {
            if (amount < 0)
            {
                throw new IncorrectAmountException();
            }
            Amount = amount;
        }

        public void PlaceOrMove(StorageLocation location)
        {
            if (!Enum.IsDefined(location) || location is StorageLocation.Unspecified)
            {
                throw new UnknownLocationException();
            }

            Location = location;
        }
    }
}
