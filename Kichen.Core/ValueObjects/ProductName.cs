using Kitchen.Core.Domain.Exceptions;

namespace Kitchen.Core.ValueObjects
{
    public class ProductName
    {
        public string Value { get; }

        public ProductName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidProductNameException();
            value = value.Trim();
            if (char.IsDigit(value[0]))
                throw new InvalidProductNameException();
            
            Value = value;
        }

        public static implicit operator ProductName(string value) => new(value);
        public static implicit operator string(ProductName name) => name.Value;
        public override string ToString() => Value;
    }
}
