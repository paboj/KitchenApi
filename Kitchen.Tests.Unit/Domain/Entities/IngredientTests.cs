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

        #region Initial

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void given_empty_name_constructor_should_fail(string name)
        {
            Action action = () => new Ingredient(name, 2, StorageLocation.Fridge);

            action.Should().Throw<InvalidIngredientNameException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void given_negative_amount_constructor_should_fail(double invalidAmount)
        {
            Action action = () => new Ingredient("Pomidor", invalidAmount, StorageLocation.Fridge);

            action.Should().Throw<IncorrectAmountException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(999)]
        public void given_invalid_location_constructor_should_fail(int invalidLocation)
        {
            Action action = () => new Ingredient("Pomidor", 2, (StorageLocation)invalidLocation);

            action.Should().Throw<UnknownLocationException>();
        }

        [Fact]
        public void given_valid_parameters_constructor_should_create_correct_entity()
        {
            // Act
            var ingredient = new Ingredient("Czosnek", 10, StorageLocation.Pantry);

            // Assert
            ingredient.Name.Value.Should().Be("Czosnek");
            ingredient.Amount.Should().Be(10);
            ingredient.Location.Should().Be(StorageLocation.Pantry);
            ingredient.Id.Should().NotBe(Guid.Empty);
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
