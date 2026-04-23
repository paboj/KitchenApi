using System.Reflection.Metadata.Ecma335;
using Kitchen.Api.Domain.Exceptions;

namespace Kitchen.Api.ValueObjects
{
    public class IngredientName
    {
        public string Value { get; }

        public IngredientName(string value) // TODO: cant start with number/ cant contain numbers?
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidIngredientNameException();
            Value = value;
        }

        public static implicit operator IngredientName(string value) => new(value);
        public static implicit operator string(IngredientName name) => name.Value;
        public override string ToString() => Value;
    }
}
