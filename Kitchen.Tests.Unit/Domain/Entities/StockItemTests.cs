using FluentAssertions;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;

namespace Kitchen.Tests.Unit.Domain.Entities
{
    public class StockItemTests
    {
        #region Arrange

        
        private readonly string _startName = "Pomidor";
        private readonly double _startAmount = 2;
        private readonly StorageLocation _startLocation = StorageLocation.Fridge;

        private readonly StockItem _StockItem;

        public StockItemTests()
        {
            _StockItem = new StockItem(_startName, _startAmount, _startLocation, null);
        }

        #endregion

        #region Initial

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void given_empty_name_constructor_should_fail(string name)
        {
            Action action = () => new StockItem(name, 2, StorageLocation.Fridge, null);

            action.Should().Throw<InvalidProductNameException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void given_negative_amount_constructor_should_fail(double invalidAmount)
        {
            Action action = () => new StockItem("Pomidor", invalidAmount, StorageLocation.Fridge, null);

            action.Should().Throw<IncorrectAmountException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(999)]
        public void given_invalid_location_constructor_should_fail(int invalidLocation)
        {
            Action action = () => new StockItem("Pomidor", 2, (StorageLocation)invalidLocation, null);

            action.Should().Throw<UnknownLocationException>();
        }

        [Fact]
        public void given_valid_parameters_constructor_should_create_correct_entity()
        {
            // Act
            var StockItem = new StockItem("Czosnek", 10, StorageLocation.Pantry, null);

            // Assert
            StockItem.Name.Value.Should().Be("Czosnek");
            StockItem.Amount.Should().Be(10);
            StockItem.Location.Should().Be(StorageLocation.Pantry);
            StockItem.Id.Should().NotBe(Guid.Empty);
        }

        #endregion

        #region AdjustAmount

        [Fact]
        public void given_correct_amount_adjustment_should_set_new_value()
        {
            var newValue = 5;

            _StockItem.AdjustAmount(newValue);

            _StockItem.Amount.Should().Be(newValue);
            _StockItem.Location.Should().Be(_startLocation);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-0.1)]
        public void given_invalid_amount_adjustment_should_fail(double invalidAmount)
        {   
            Action action = () => _StockItem.AdjustAmount(invalidAmount);

            action.Should().Throw<IncorrectAmountException>();
        }

        [Fact]
        public void given_null_amount_adjustment_should_leave_previous_value()
        {
            _StockItem.AdjustAmount(null);

            _StockItem.Amount.Should().Be(_startAmount);
            _StockItem.Location.Should().Be(_startLocation);
        }

        #endregion

        #region PlaceOrMove

        [Fact]
        public void given_valid_location_place_or_move_should_update_location()
        {
            var newLocation = StorageLocation.Pantry;

            _StockItem.PlaceOrMove(newLocation);

            _StockItem.Location.Should().Be(newLocation);
            _StockItem.Amount.Should().Be(_startAmount);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(666)]
        public void given_invalid_location_place_or_move_should_fail(int invalidLocation)
        {
            Action action = () => _StockItem.PlaceOrMove((StorageLocation)invalidLocation);

            action.Should().Throw<UnknownLocationException>();
        }

        [Fact]
        public void given_null_location_place_or_move_should_leave_previous_value()
        {
            _StockItem.PlaceOrMove(null);

            _StockItem.Amount.Should().Be(_startAmount);
            _StockItem.Location.Should().Be(_startLocation);
        }

        #endregion

    }
}
