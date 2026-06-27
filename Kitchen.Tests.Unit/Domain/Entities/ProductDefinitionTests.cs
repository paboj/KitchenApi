using FluentAssertions;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;

namespace Kitchen.Tests.Unit.Domain.Entities
{
    public class ProductDefinitionTests
    {

        #region Arrange

        private readonly ProductDefinition _ProductDefinition;

        public ProductDefinitionTests()
        {
            _ProductDefinition = new ProductDefinition("Mąka", UnitType.Kilograms, Category.DryGoods);
        }

        #endregion

        #region Initial

        [Fact]
        public void GivenValidParameters_Constructor_ShouldCreateCorrectEntity()
        {
            // Act
            var type = new ProductDefinition("Mleko", UnitType.Liters, Category.Dairy);

            // Assert
            type.Name.Value.Should().Be("Mleko");
            type.Unit.Should().Be(UnitType.Liters);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GivenEmptyName_Constructor_ShouldFail(string invalidName)
        {
            Action action = () => new ProductDefinition(invalidName, UnitType.Kilograms, Category.DryGoods);

            action.Should().Throw<InvalidProductNameException>();
        }

        [Theory]
        [InlineData((UnitType)(-1))]
        [InlineData((UnitType)999)]
        public void GivenInvalidUnit_Constructor_ShouldFail(UnitType invalidUnit)
        {
            Action action = () => new ProductDefinition("Mąka", invalidUnit, Category.DryGoods);

            action.Should().Throw<UnknownUnitTypeException>();
        }

        #endregion

        #region ChangeUnitType

        [Fact]
        public void ChangeUnitType_ShouldUpdateUnit_WhenValid()
        {
            var newUnit = UnitType.Liters;

            _ProductDefinition.ChangeUnitType(newUnit);

            _ProductDefinition.Unit.Should().Be(newUnit);
        }

        [Theory]
        [InlineData((UnitType)66)]
        public void ChangeUnitType_ShouldThrowException_WhenInvalid(UnitType invalidUnit)
        {
            Action action = () => _ProductDefinition.ChangeUnitType(invalidUnit);

            action.Should().Throw<UnknownUnitTypeException>();
        }

        #endregion
    }
}