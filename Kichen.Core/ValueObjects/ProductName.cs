using Kitchen.Core.Domain.Exceptions;

namespace Kitchen.Core.ValueObjects
{
    public class ProductName : IEquatable<ProductName>
    {
        public string Value { get; }

        public ProductName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidProductNameException();
            value = value.Trim();
            if (char.IsDigit(value[0]))
                throw new InvalidProductNameException();

            Value = value.ToLowerInvariant();
        }

        public static implicit operator ProductName(string value) => new(value);
        public static implicit operator string(ProductName name) => name.Value;
        public override string ToString() => Value;

        public bool Equals(ProductName? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj) => Equals(obj as ProductName);

        public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

        public static bool operator ==(ProductName? left, ProductName? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(ProductName? left, ProductName? right) => !(left == right);
    }
}
