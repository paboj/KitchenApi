using FluentAssertions;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Tests.Unit.Domain.ValueObjects
{
    public class ProductNameTests
    {
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
