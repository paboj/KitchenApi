using FluentAssertions;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;

namespace Kitchen.Tests.Unit.Domain.Entities
{
    public class IngredientTests
    {
        #region Arrange

        private readonly Ingredient _ingredient;

        public IngredientTests()
        {
            _ingredient = new Ingredient("Pomidor", 2, StorageLocation.Fridge);
        }

        #endregion

        #region AdjustAmount

        [Fact]
        public void given_correct_amount_adjustment_should_set_new_value()
        {
            var newValue = 5;

            _ingredient.AdjustAmount(newValue);

            _ingredient.Amount.Should().Be(newValue);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-0.1)]
        public void given_invalid_amount_adjustment_should_fail(double invalidAmount)
        {   
            Action action = () => _ingredient.AdjustAmount(invalidAmount);

            action.Should().Throw<IncorrectAmountException>();
        }

        #endregion

        #region PlaceOrMove

        [Fact]
        public void given_valid_location_place_or_move_should_update_location()
        {
            var newLocation = StorageLocation.Pantry;

            _ingredient.PlaceOrMove(newLocation);

            _ingredient.Location.Should().Be(newLocation);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(666)]
        public void given_invalid_location_place_or_move_should_fail(int invalidLocation)
        {
            Action action = () => _ingredient.PlaceOrMove((StorageLocation)invalidLocation);

            action.Should().Throw<UnknownLocationException>();
        }

        #endregion

    }
}
