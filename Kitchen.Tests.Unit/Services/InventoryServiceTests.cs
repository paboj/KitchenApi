using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitchen.Api.Domain.Entities;
using Kitchen.Api.Domain.Enums;
using Kitchen.Api.Repositories;
using Kitchen.Api.ValueObjects;
using Moq;
using Xunit;

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
                .Setup(repo => repo.GetByName(It.IsAny<IngredientName>()))
                .Returns(expectedIngredient);

            // Act
            var result = _service.GetByName(ingredientName);

            // Assert 
            Assert.NotNull(result);
            Assert.Equal(ingredientName, (string)result.Name);

            // Optional
            _repositoryMock.Verify(repo => repo.GetByName(It.IsAny<IngredientName>()), Times.Once);
        }

        [Fact]
        public void GetByName_ShouldReturnNull_WhenIngredientDoesNotExist()
        {
            // Arrange
            _repositoryMock
                .Setup(repo => repo.GetByName(It.IsAny<IngredientName>()))
                .Returns((Ingredient?)null);

            // Act
            var result = _service.GetByName("NonExistent");

            // Assert
            Assert.Null(result);
        }
    }
}
