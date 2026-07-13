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
        public void GivenEmptyName_Constructor_ShouldFail(string name)
        {
            Action action = () => new StockItem(name, 2, StorageLocation.Fridge, null);

            action.Should().Throw<InvalidProductNameException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void GivenNegativeAmount_Constructor_ShouldFail(double invalidAmount)
        {
            Action action = () => new StockItem("Pomidor", invalidAmount, StorageLocation.Fridge, null);

            action.Should().Throw<IncorrectAmountException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(999)]
        public void GivenInvalidLocation_Constructor_ShouldFail(int invalidLocation)
        {
            Action action = () => new StockItem("Pomidor", 2, (StorageLocation)invalidLocation, null);

            action.Should().Throw<UnknownLocationException>();
        }

        [Fact]
        public void GivenValidParameters_Constructor_ShouldCreateCorrectEntity()
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
        public void GivenCorrectAmount_Adjustment_ShouldSetNewValue()
        {
            var newValue = 5;

            _StockItem.AdjustAmount(newValue);

            _StockItem.Amount.Should().Be(newValue);
            _StockItem.Location.Should().Be(_startLocation);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-0.1)]
        public void GivenInvalidAmount_Adjustment_ShouldFail(double invalidAmount)
        {   
            Action action = () => _StockItem.AdjustAmount(invalidAmount);

            action.Should().Throw<IncorrectAmountException>();
        }

        [Fact]
        public void GivenNullAmount_Adjustment_ShouldLeavePreviousValue()
        {
            _StockItem.AdjustAmount(null);

            _StockItem.Amount.Should().Be(_startAmount);
            _StockItem.Location.Should().Be(_startLocation);
        }

        #endregion

        #region PlaceOrMove

        [Fact]
        public void GivenValidLocation_PlaceOrMove_ShouldUpdateLocation()
        {
            var newLocation = StorageLocation.Pantry;

            _StockItem.PlaceOrMove(newLocation);

            _StockItem.Location.Should().Be(newLocation);
            _StockItem.Amount.Should().Be(_startAmount);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(666)]
        public void GivenInvalidLocation_PlaceOrMove_ShouldFail(int invalidLocation)
        {
            Action action = () => _StockItem.PlaceOrMove((StorageLocation)invalidLocation);

            action.Should().Throw<UnknownLocationException>();
        }

        [Fact]
        public void GivenNullLocation_PlaceOrMove_ShouldLeavePreviousValue()
        {
            _StockItem.PlaceOrMove(null);

            _StockItem.Amount.Should().Be(_startAmount);
            _StockItem.Location.Should().Be(_startLocation);
        }

        #endregion

        #region SetName

        [Fact]
        public void GivenValidName_SetName_ShouldUpdateName()
        {
            _StockItem.SetName("Ogórek");

            _StockItem.Name.Value.Should().Be("Ogórek");
        }

        [Fact]
        public void GivenNullName_SetName_ShouldLeavePreviousValue()
        {
            _StockItem.SetName(null);

            _StockItem.Name.Value.Should().Be(_startName);
        }

        [Fact]
        public void GivenEmptyName_SetName_ShouldFail()
        {
            Action action = () => _StockItem.SetName("");

            action.Should().Throw<InvalidProductNameException>();
        }

        #endregion

        #region AssignDefinition

        [Fact]
        public void GivenDefinition_AssignDefinition_ShouldSetDefinitionAndDefinitionName()
        {
            var definition = new ProductDefinition(_startName, UnitType.Kilograms, Category.Vegetables);

            _StockItem.AssignDefinition(definition);

            _StockItem.Definition.Should().Be(definition);
            _StockItem.DefinitionName.Should().Be(definition.Name);
        }

        [Fact]
        public void GivenNullDefinition_AssignDefinition_ShouldLeaveDefinitionUnset()
        {
            _StockItem.AssignDefinition(null);

            _StockItem.Definition.Should().BeNull();
            _StockItem.DefinitionName.Should().BeNull();
        }

        #endregion

        #region SetExpirationDate

        [Fact]
        public void GivenDate_SetExpirationDate_ShouldUpdateValue()
        {
            var date = new DateOnly(2026, 12, 31);

            _StockItem.SetExpirationDate(date);

            _StockItem.ExpirationDate.Should().Be(date);
        }

        [Fact]
        public void GivenNullDate_SetExpirationDate_ShouldLeavePreviousValue()
        {
            var date = new DateOnly(2026, 12, 31);
            _StockItem.SetExpirationDate(date);

            _StockItem.SetExpirationDate(null);

            _StockItem.ExpirationDate.Should().Be(date);
        }

        [Fact]
        public void GivenPastDate_SetExpirationDate_ShouldStillSetValue()
        {
            // Intentionally allowed: an expired item is still a real item in the fridge.
            var pastDate = new DateOnly(2020, 1, 1);

            _StockItem.SetExpirationDate(pastDate);

            _StockItem.ExpirationDate.Should().Be(pastDate);
        }

        #endregion

    }
}
