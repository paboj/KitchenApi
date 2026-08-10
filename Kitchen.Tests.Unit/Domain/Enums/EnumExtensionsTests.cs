using System.ComponentModel;
using AwesomeAssertions;
using Kitchen.Core.Domain.Enums;

namespace Kitchen.Tests.Unit.Domain.Enums
{
    public class EnumExtensionsTests
    {
        private enum EnumWithoutDescription
        {
            SomeValue = 0
        }

        private enum EnumWithDescription
        {
            [Description("opisana wartość")]
            SomeValue = 0
        }

        [Fact]
        public void ToDescription_ShouldReturnAttributeValue_WhenDescriptionAttributePresent()
        {
            UnitType.Kilograms.ToDescription().Should().Be("kg");
        }

        [Fact]
        public void ToDescription_ShouldReturnAttributeValue_ForLocalEnum()
        {
            EnumWithDescription.SomeValue.ToDescription().Should().Be("opisana wartość");
        }

        [Fact]
        public void ToDescription_ShouldFallBackToToString_WhenDescriptionAttributeMissing()
        {
            EnumWithoutDescription.SomeValue.ToDescription().Should().Be(nameof(EnumWithoutDescription.SomeValue));
        }
    }
}
