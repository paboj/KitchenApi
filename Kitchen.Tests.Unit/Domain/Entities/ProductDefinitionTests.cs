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
        public void given_valid_parameters_constructor_should_create_correct_entity()
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
        public void given_empty_name_constructor_should_fail(string invalidName)
        {
            Action action = () => new ProductDefinition(invalidName, UnitType.Kilograms, Category.DryGoods);

            action.Should().Throw<InvalidProductNameException>();
        }

        [Theory]
        [InlineData((UnitType)(-1))]
        [InlineData((UnitType)999)]
        public void given_invalid_unit_constructor_should_fail(UnitType invalidUnit)
        {
            Action action = () => new ProductDefinition("Mąka", invalidUnit, Category.DryGoods);

            action.Should().Throw<UnknownUnitTypeException>();
        }

        #endregion

        #region ChangeUnitType

        [Fact]
        public void change_unit_type_should_update_unit_when_valid()
        {
            var newUnit = UnitType.Liters;

            _ProductDefinition.ChangeUnitType(newUnit);

            _ProductDefinition.Unit.Should().Be(newUnit);
        }

        [Theory]
        [InlineData((UnitType)66)]
        public void change_unit_type_should_throw_exception_when_invalid(UnitType invalidUnit)
        {
            Action action = () => _ProductDefinition.ChangeUnitType(invalidUnit);

            action.Should().Throw<UnknownUnitTypeException>();
        }

        #endregion
    }
}