using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Domain.Entities
{
    public class StockItem
    {
        public StockItemId Id { get; private set; } = null!;
        public ProductName Name { get; private set; } = null!;
        public double Amount { get; private set; } = 0;

        public StorageLocation Location { get; private set; } = StorageLocation.Unspecified;
        public ProductName? DefinitionName { get; private set; }
        public ProductDefinition? Definition { get; private set; }
        public DateOnly? ExpirationDate { get; private set; }

        private StockItem() { }

        public StockItem(string name, double amount, StorageLocation location, ProductDefinition? definition, DateOnly? expirationDate = null) {
            Id = new StockItemId(Guid.NewGuid());
            Name = new ProductName(name);
            AdjustAmount(amount);
            PlaceOrMove(location);
            AssignDefinition(definition);
            SetExpirationDate(expirationDate);
        }

        public void SetName(string? name)
        {
            if (name is null) return;
            Name = new ProductName(name);
        }

        public void AssignDefinition(ProductDefinition? definition)
        {
            if (definition != null)
            {
                Definition = definition;
                DefinitionName = definition.Name;
            }
        }

        // Intentionally no "not in the past" validation: an expired item is still
        // a real item in the fridge, just one you should eat soon or throw out.
        public void SetExpirationDate(DateOnly? expirationDate)
        {
            if (expirationDate is null) return;
            ExpirationDate = expirationDate;
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
