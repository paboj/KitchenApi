using AwesomeAssertions;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Tests.Unit.Domain.Entities
{
    public class ProductDefinitionTests
    {

        #region Arrange

        private const string ValidName = "Mąka";
        private const UnitType ValidUnitType = UnitType.Kilograms;
        private const Category ValidCategory = Category.Dairy;

        private readonly ProductDefinition _productDefinition;

        public ProductDefinitionTests()
        {
            _productDefinition = new ProductDefinition(ValidName, ValidUnitType, ValidCategory);
        }

        #endregion

        #region Initial

        [Fact]
        public void GivenValidParameters_Constructor_ShouldCreateCorrectEntity()
        {
            // Act
            var definition = new ProductDefinition(ValidName, ValidUnitType, ValidCategory);

            // Assert
            definition.Name.Value.Should().Be(new ProductName(ValidName));
            definition.Unit.Should().Be(ValidUnitType);
            definition.Category.Should().Be(ValidCategory);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GivenEmptyName_Constructor_ShouldFail(string invalidName)
        {
            Action action = () => new ProductDefinition(invalidName, ValidUnitType, ValidCategory);

            action.Should().Throw<InvalidProductNameException>();
        }

        [Theory]
        [InlineData((UnitType)(-1))]
        [InlineData((UnitType)999)]
        public void GivenInvalidUnit_Constructor_ShouldFail(UnitType invalidUnit)
        {
            Action action = () => new ProductDefinition(ValidName, invalidUnit, ValidCategory);

            action.Should().Throw<UnknownUnitTypeException>();
        }

        #endregion

        #region ChangeUnitType

        [Fact]
        public void ChangeUnitType_ShouldUpdateUnit_WhenValid()
        {
            var newUnit = UnitType.Liters;

            _productDefinition.Unit.Should().NotBe(newUnit);

            _productDefinition.ChangeUnitType(newUnit);

            _productDefinition.Unit.Should().Be(newUnit);
        }

        [Theory]
        [InlineData((UnitType)66)]
        public void ChangeUnitType_ShouldThrowException_WhenInvalid(UnitType invalidUnit)
        {
            Action action = () => _productDefinition.ChangeUnitType(invalidUnit);

            action.Should().Throw<UnknownUnitTypeException>();
        }

        [Fact]
        public void ChangeUnitType_ShouldLeavePreviousValue_WhenNull()
        {
            var startingUnit = _productDefinition.Unit;

            _productDefinition.ChangeUnitType(null);

            _productDefinition.Unit.Should().Be(startingUnit);
        }

        #endregion

        #region SetCategory

        [Fact]
        public void SetCategory_ShouldUpdateCategory_WhenValid()
        {
            var newCategory = Category.Spices;

            _productDefinition.SetCategory(newCategory);

            _productDefinition.Category.Should().Be(newCategory);
        }

        [Fact]
        public void SetCategory_ShouldLeavePreviousValue_WhenNull()
        {
            var startingCategory = _productDefinition.Category;

            _productDefinition.SetCategory(null);

            _productDefinition.Category.Should().Be(startingCategory);
        }

        [Theory]
        [InlineData((Category)(-1))]
        [InlineData((Category)999)]
        public void SetCategory_ShouldThrowException_WhenInvalid(Category invalidCategory)
        {
            Action action = () => _productDefinition.SetCategory(invalidCategory);

            action.Should().Throw<UnknownCategoryException>();
        }

        #endregion
    }
}