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