using FluentAssertions;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Tests.Unit.Domain.ValueObjects
{
    public class ProductNameTests
    {
        [Theory]
        [InlineData("1 Mleko")]
        [InlineData("9")]
        public void Constructor_ShouldThrow_WhenNameStartsWithDigit(string invalidName)
        {
            Action action = () => new ProductName(invalidName);

            action.Should().Throw<InvalidProductNameException>();
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenNameIsWhitespaceOnly()
        {
            Action action = () => new ProductName("   ");

            action.Should().Throw<InvalidProductNameException>();
        }

        [Fact]
        public void Constructor_ShouldTrimValue()
        {
            var name = new ProductName("  Mleko  ");

            name.Value.Should().Be("Mleko");
        }

        [Fact]
        public void ImplicitConversion_FromString_ShouldCreateProductName()
        {
            ProductName name = "Mleko";

            name.Value.Should().Be("Mleko");
        }

        [Fact]
        public void ImplicitConversion_ToString_ShouldReturnValue()
        {
            var name = new ProductName("Mleko");

            string value = name;

            value.Should().Be("Mleko");
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            var name = new ProductName("Mleko");

            name.ToString().Should().Be("Mleko");
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenComparedToDifferentType()
        {
            var name = new ProductName("Mleko");

            name.Equals((object)"Mleko").Should().BeFalse();
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenValuesAreTheSame_EvenForDifferentInstances()
        {
            var first = new ProductName("Mleko waniliowe");
            var second = new ProductName("Mleko waniliowe");

            first.Equals(second).Should().BeTrue();
            (first == second).Should().BeTrue();
            (first != second).Should().BeFalse();
            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenValuesDiffer()
        {
            var first = new ProductName("Mleko waniliowe");
            var second = new ProductName("Mleko");

            first.Equals(second).Should().BeFalse();
            (first == second).Should().BeFalse();
            (first != second).Should().BeTrue();
        }

        [Fact]
        public void EqualityOperator_ShouldHandleNulls()
        {
            ProductName? left = null;
            ProductName? right = null;

            (left == right).Should().BeTrue();

            right = new ProductName("Mleko");

            (left == right).Should().BeFalse();
            (right == left).Should().BeFalse();
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenComparedToNull()
        {
            var name = new ProductName("Mleko");

            name.Equals(null).Should().BeFalse();
        }
    }
}
