using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Domain.Entities
{
    public class StockItem
    {
        public StockItemId Id { get; private set; }
        public ProductName Name { get; set; }
        public double Amount { get; private set; } = 0;
        public StorageLocation Location { get; private set; } = StorageLocation.Unspecified;
        public ProductName? TypeName { get; private set; }
        public ProductDefinition? Type { get; private set; }

        private StockItem() { }

        public StockItem(string name, double amount, StorageLocation location, ProductDefinition? type) { 
            Id = new StockItemId(Guid.NewGuid());
            Name = name;
            AdjustAmount(amount);
            PlaceOrMove(location);
            AssignType(type);
        }

        public void AssignType(ProductDefinition? type)
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
