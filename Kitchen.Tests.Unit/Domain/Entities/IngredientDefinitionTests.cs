using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Kitchen.Api.Domain.Entities;
using Kitchen.Api.Domain.Enums;
using Kitchen.Api.Domain.Exceptions;

namespace Kitchen.Tests.Unit.Domain.Entities
{
    public class IngredientDefinitionTests
    {

        #region Arrange

        private readonly IngredientDefinition _ingredientDefinition;

        public IngredientDefinitionTests()
        {
            _ingredientDefinition = new IngredientDefinition("Mąka", UnitType.Grams);
        }

        #endregion

        [Fact]
        public void change_unit_type_should_update_unit_when_valid()
        {
            var newUnit = UnitType.Milliliters;

            _ingredientDefinition.ChangeUnitType(newUnit);

            _ingredientDefinition.Unit.Should().Be(newUnit);
        }

        [Theory]
        [InlineData(UnitType.Unspecified)]
        [InlineData((UnitType)66)]
        public void change_unit_type_should_throw_exception_when_invalid(UnitType invalidUnit)
        {
            Action action = () => _ingredientDefinition.ChangeUnitType(invalidUnit);

            action.Should().Throw<UnknownUnitTypeException>();
        }
    }
}