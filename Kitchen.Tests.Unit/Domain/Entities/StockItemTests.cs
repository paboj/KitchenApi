using FluentAssertions;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Tests.Unit.Domain.Entities
{
    public class StockItemTests
    {
        #region Arrange

        
        private const string ValidName = "Pomidor";
        private const double ValidAmount = 2;
        private const StorageLocation ValidLocation = StorageLocation.Fridge;

        private const UnitType ValidUnitType = UnitType.Pieces;
        private const Category ValidCategory = Category.Vegetables;

        private readonly ProductDefinition _productDefinition;
        private readonly StockItem _stockItem;

        public StockItemTests()
        {
            _productDefinition = new ProductDefinition(ValidName, ValidUnitType, ValidCategory);
            _stockItem = new StockItem(ValidName, ValidAmount, ValidLocation, null);
        }

        #endregion

        #region Initial

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GivenEmptyName_Constructor_ShouldFail(string name)
        {
            Action action = () => new StockItem(name, ValidAmount, ValidLocation, null);

            action.Should().Throw<InvalidProductNameException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void GivenNegativeAmount_Constructor_ShouldFail(double invalidAmount)
        {
            Action action = () => new StockItem(ValidName, invalidAmount, ValidLocation, null);

            action.Should().Throw<IncorrectAmountException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(999)]
        public void GivenInvalidLocation_Constructor_ShouldFail(int invalidLocation)
        {
            Action action = () => new StockItem(ValidName, ValidAmount, (StorageLocation)invalidLocation, null);

            action.Should().Throw<UnknownLocationException>();
        }

        [Fact]
        public void GivenValidParameters_Constructor_ShouldCreateCorrectEntity()
        {
            // Act
            var StockItem = new StockItem(ValidName, ValidAmount, ValidLocation, _productDefinition);

            // Assert
            StockItem.Id.Should().NotBe(Guid.Empty);
            StockItem.Name.Value.Should().Be(new ProductName(ValidName));
            StockItem.Amount.Should().Be(ValidAmount);
            StockItem.Location.Should().Be(ValidLocation);
            StockItem.Definition.Should().Be(_productDefinition);
            
        }

        #endregion

        #region AdjustAmount

        [Fact]
        public void GivenCorrectAmount_Adjustment_ShouldSetNewValue()
        {
            var newValue = 5;
            var startLocation = _stockItem.Location;

            _stockItem.AdjustAmount(newValue);

            _stockItem.Amount.Should().Be(newValue);
            _stockItem.Location.Should().Be(startLocation);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-0.1)]
        public void GivenInvalidAmount_Adjustment_ShouldFail(double invalidAmount)
        {   
            Action action = () => _stockItem.AdjustAmount(invalidAmount);

            action.Should().Throw<IncorrectAmountException>();
        }

        [Fact]
        public void GivenNullAmount_Adjustment_ShouldLeavePreviousValue()
        {
            var startAmount = _stockItem.Amount;
            var startLocation = _stockItem.Location;

            _stockItem.AdjustAmount(null);

            _stockItem.Amount.Should().Be(startAmount);
            _stockItem.Location.Should().Be(startLocation);
        }

        #endregion

        #region PlaceOrMove

        [Fact]
        public void GivenValidLocation_PlaceOrMove_ShouldUpdateLocation()
        {
            var newLocation = StorageLocation.Pantry;
            var startAmount = _stockItem.Amount;

            _stockItem.PlaceOrMove(newLocation);

            _stockItem.Location.Should().Be(newLocation);
            _stockItem.Amount.Should().Be(startAmount);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(666)]
        public void GivenInvalidLocation_PlaceOrMove_ShouldFail(int invalidLocation)
        {
            Action action = () => _stockItem.PlaceOrMove((StorageLocation)invalidLocation);

            action.Should().Throw<UnknownLocationException>();
        }

        [Fact]
        public void GivenNullLocation_PlaceOrMove_ShouldLeavePreviousValue()
        {
            var startAmount = _stockItem.Amount;
            var startLocation = _stockItem.Location;

            _stockItem.PlaceOrMove(null);

            _stockItem.Amount.Should().Be(startAmount);
            _stockItem.Location.Should().Be(startLocation);
        }

        #endregion

        #region SetName

        [Fact]
        public void GivenValidName_SetName_ShouldUpdateName()
        {
            _stockItem.SetName("Ogórek");

            _stockItem.Name.Value.Should().Be(new ProductName("Ogórek"));
        }

        [Fact]
        public void GivenNullName_SetName_ShouldLeavePreviousValue()
        {
            var startName = _stockItem.Name;

            _stockItem.SetName(null);

            _stockItem.Name.Value.Should().Be(startName);
        }

        [Fact]
        public void GivenEmptyName_SetName_ShouldFail()
        {
            Action action = () => _stockItem.SetName("");

            action.Should().Throw<InvalidProductNameException>();
        }

        #endregion

        #region AssignDefinition

        [Fact]
        public void GivenDefinition_AssignDefinition_ShouldSetDefinitionAndDefinitionName()
        {
            var definition = new ProductDefinition(ValidName, ValidUnitType, ValidCategory);

            _stockItem.AssignDefinition(definition);

            _stockItem.Definition.Should().Be(definition);
            _stockItem.DefinitionName.Should().Be(definition.Name);
        }

        [Fact]
        public void GivenNullDefinition_AssignDefinition_ShouldLeaveDefinitionUnset()
        {
            _stockItem.AssignDefinition(null);

            _stockItem.Definition.Should().BeNull();
            _stockItem.DefinitionName.Should().BeNull();
        }

        #endregion

        #region SetExpirationDate

        [Fact]
        public void GivenDate_SetExpirationDate_ShouldUpdateValue()
        {
            var date = new DateOnly(2026, 12, 31);

            _stockItem.SetExpirationDate(date);

            _stockItem.ExpirationDate.Should().Be(date);
        }

        [Fact]
        public void GivenNullDate_SetExpirationDate_ShouldLeavePreviousValue()
        {
            var date = new DateOnly(2026, 12, 31);
            _stockItem.SetExpirationDate(date);

            _stockItem.SetExpirationDate(null);

            _stockItem.ExpirationDate.Should().Be(date);
        }

        [Fact]
        public void GivenPastDate_SetExpirationDate_ShouldStillSetValue()
        {
            // Intentionally allowed: an expired item is still a real item in the fridge.
            var pastDate = new DateOnly(2020, 1, 1);

            _stockItem.SetExpirationDate(pastDate);

            _stockItem.ExpirationDate.Should().Be(pastDate);
        }

        #endregion

    }
}
