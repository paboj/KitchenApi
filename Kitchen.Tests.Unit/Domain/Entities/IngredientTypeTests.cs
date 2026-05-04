using FluentAssertions;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;

namespace Kitchen.Tests.Unit.Domain.Entities
{
    public class IngredientTypeTests
    {

        #region Arrange

        private readonly IngredientType _IngredientType;

        public IngredientTypeTests()
        {
            _IngredientType = new IngredientType("Mąka", UnitType.Grams);
        }

        #endregion

        #region Initial

        [Fact]
        public void given_valid_parameters_constructor_should_create_correct_entity()
        {
            // Act
            var type = new IngredientType("Mleko", UnitType.Milliliters);

            // Assert
            type.Name.Value.Should().Be("Mleko");
            type.Unit.Should().Be(UnitType.Milliliters);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void given_empty_name_constructor_should_fail(string invalidName)
        {
            Action action = () => new IngredientType(invalidName, UnitType.Grams);

            action.Should().Throw<InvalidIngredientNameException>();
        }

        [Theory]
        [InlineData((UnitType)(-1))]
        [InlineData((UnitType)999)]
        public void given_invalid_unit_constructor_should_fail(UnitType invalidUnit)
        {
            Action action = () => new IngredientType("Mąka", invalidUnit);

            action.Should().Throw<UnknownUnitTypeException>();
        }

        #endregion

        #region ChangeUnitType

        [Fact]
        public void change_unit_type_should_update_unit_when_valid()
        {
            var newUnit = UnitType.Milliliters;

            _IngredientType.ChangeUnitType(newUnit);

            _IngredientType.Unit.Should().Be(newUnit);
        }

        [Theory]
        [InlineData((UnitType)66)]
        public void change_unit_type_should_throw_exception_when_invalid(UnitType invalidUnit)
        {
            Action action = () => _IngredientType.ChangeUnitType(invalidUnit);

            action.Should().Throw<UnknownUnitTypeException>();
        }

        #endregion
    }
}