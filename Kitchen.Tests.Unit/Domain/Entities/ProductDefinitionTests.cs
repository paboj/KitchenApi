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

        [Fact]
        public void ChangeUnitType_ShouldLeavePreviousValue_WhenNull()
        {
            var startingUnit = _ProductDefinition.Unit;

            _ProductDefinition.ChangeUnitType(null);

            _ProductDefinition.Unit.Should().Be(startingUnit);
        }

        #endregion

        #region SetName

        [Fact]
        public void SetName_ShouldUpdateName_WhenValid()
        {
            _ProductDefinition.SetName("Kasza");

            _ProductDefinition.Name.Value.Should().Be("Kasza");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetName_ShouldThrowException_WhenInvalid(string invalidName)
        {
            Action action = () => _ProductDefinition.SetName(invalidName);

            action.Should().Throw<InvalidProductNameException>();
        }

        #endregion

        #region SetCategory

        [Fact]
        public void SetCategory_ShouldUpdateCategory_WhenValid()
        {
            var newCategory = Category.Spices;

            _ProductDefinition.SetCategory(newCategory);

            _ProductDefinition.Category.Should().Be(newCategory);
        }

        [Fact]
        public void SetCategory_ShouldLeavePreviousValue_WhenNull()
        {
            var startingCategory = _ProductDefinition.Category;

            _ProductDefinition.SetCategory(null);

            _ProductDefinition.Category.Should().Be(startingCategory);
        }

        [Theory]
        [InlineData((Category)(-1))]
        [InlineData((Category)999)]
        public void SetCategory_ShouldThrowException_WhenInvalid(Category invalidCategory)
        {
            Action action = () => _ProductDefinition.SetCategory(invalidCategory);

            action.Should().Throw<UnknownCategoryException>();
        }

        #endregion
    }
}