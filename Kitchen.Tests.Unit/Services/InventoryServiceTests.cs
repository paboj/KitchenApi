using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.Repositories;
using Kitchen.Core.ValueObjects;
using Moq;

namespace Kitchen.Tests.Unit.Services
{
    public class InventoryServiceTests
    {
        private readonly Mock<IIngredientRepository> _repositoryMock;
        private readonly InventoryService _service;

        public InventoryServiceTests()
        {
            _repositoryMock = new Mock<IIngredientRepository>();
            _service = new InventoryService(_repositoryMock.Object);
        }

        [Fact]
        public void GetByName_ShouldReturnIngredient_WhenIngredientExists()
        {
            // Arrange 
            var ingredientName = "Onion";
            var expectedIngredient = new Ingredient(ingredientName, 10, StorageLocation.Fridge);

            _repositoryMock
                .Setup(repo => repo.GetByName(ingredientName))
                .Returns(expectedIngredient);

            // Act
            var result = _service.GetByName(ingredientName);

            // Assert 
            Assert.NotNull(result);
            Assert.Equal(ingredientName, (string)result.Name);

            // Optional
            _repositoryMock.Verify(repo => repo.GetByName(ingredientName), Times.Once);
        }

        [Fact]
        public void GetByName_ShouldReturnNull_WhenIngredientDoesNotExist()
        {
            // Arrange
            _repositoryMock
                .Setup(repo => repo.GetByName(It.IsAny<string>()))
                .Returns((Ingredient?)null);

            // Act
            var result = _service.GetByName("NonExistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnIngredients_WhenIngredientsExists()
        {
            // Arrange 
            IEnumerable<Ingredient> expectedIngredients = new List<Ingredient>
            {
                new Ingredient("Onion", 10, StorageLocation.Fridge),
                new Ingredient("Potato", 5, StorageLocation.Pantry)
            };

            _repositoryMock
                .Setup(repo => repo.GetAll())
                .Returns(expectedIngredients);

            // Act
            var result = _service.GetAll();

            // Assert 
            Assert.NotNull(result);
            Assert.Equal(expectedIngredients, result);

            _repositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void Add_ShouldSucceed_WhenValidIngredient()
        {
            var command = new AddToStockCommand("Onion", 10, StorageLocation.Fridge);

            _service.Add(command);

            _repositoryMock.Verify(repo => repo.Add(It.Is<Ingredient>(i =>
                i.Name == command.Name &&
                i.Amount == command.Amount &&
                i.Location == command.Location)),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldSucceed_WhenValidIngredient()
        {
            var ingredientName = "Onion";
            var initialAmount = 10;
            var newAmount = 5;
            var initialLocation = StorageLocation.Fridge;
            var newLocation = StorageLocation.Pantry;

            var existingIngredient = new Ingredient(ingredientName, initialAmount, initialLocation);

            _repositoryMock
                .Setup(repo => repo.GetByName(ingredientName))
                .Returns(existingIngredient);

            var command = new ModifyInStockCommand(ingredientName, newAmount, newLocation);

            _service.Update(command);

            Assert.Equal(newAmount, existingIngredient.Amount);
            Assert.Equal(newLocation, existingIngredient.Location);

        }

        [Fact]
        public void Delete_ShouldCallRepository_WhenIngredientExists()
        {
            var ingredientName = "Onion";
            var existingIngredient = new Ingredient(ingredientName, 10, StorageLocation.Fridge);

            _repositoryMock
                .Setup(repo => repo.GetByName(ingredientName))
                .Returns(existingIngredient);

            _service.Delete(ingredientName);

            _repositoryMock.Verify(repo => repo.Delete(ingredientName), Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenIngredientDoesNotExists()
        {
            var ingredientName = "Onion";

            _repositoryMock
                .Setup(repo => repo.GetByName(ingredientName))
                .Returns((Ingredient?)null);

            var action = () => _service.Delete(ingredientName);

            Assert.Throws<IngredientNotFoundException>(action);

            _repositoryMock.Verify(repo => repo.Delete(It.IsAny<string>()), Times.Never);
        }
    }
}
