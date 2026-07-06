using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.Repositories;
using Moq;

namespace Kitchen.Tests.Unit.Services
{
    public class CatalogServiceTests
    {
        private readonly Mock<IProductDefinitionRepository> _productDefinitionRepositoryMock;
        private readonly Mock<IStockItemRepository> _stockItemRepositoryMock;
        private readonly CatalogService _service;

        public CatalogServiceTests()
        {
            _productDefinitionRepositoryMock = new Mock<IProductDefinitionRepository>();
            _stockItemRepositoryMock = new Mock<IStockItemRepository>();
            _service = new CatalogService(_productDefinitionRepositoryMock.Object, _stockItemRepositoryMock.Object);
        }

        [Fact]
        public async Task Add_ShouldThrow_WhenProductDefinitionAlreadyExists()
        {
            var command = new AddProductDefinitionCommand("Mleko waniliowe", UnitType.Liters, Category.Dairy);

            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetByName(command.Name))
                .ReturnsAsync(new ProductDefinition(command.Name, command.Unit, command.Category));

            var action = async () => await _service.Add(command);

            await Assert.ThrowsAsync<ProductDefinitionAlreadyExistsException>(action);

            _stockItemRepositoryMock.Verify(repo => repo.Update(It.IsAny<StockItem>()), Times.Never);
        }

        // Regression test for a bug where StockItems added before their ProductDefinition
        // existed never got linked: ProductName had no value equality, so the name
        // comparison in LinkToExistingStockItems always compared by reference and failed
        // even when the names matched textually.
        [Fact]
        public async Task Add_ShouldLinkMatchingStockItem_WhenAddedBeforeProductDefinitionExisted()
        {
            var stockItemName = "Mleko waniliowe";
            var command = new AddProductDefinitionCommand(stockItemName, UnitType.Liters, Category.Dairy);

            // StockItem and ProductDefinition each construct their own ProductName instance
            // internally, so this only passes if ProductName compares by value, not by reference.
            var unlinkedStockItem = new StockItem(stockItemName, 1, StorageLocation.Fridge, null);

            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetByName(stockItemName))
                .ReturnsAsync((ProductDefinition?)null);

            _stockItemRepositoryMock
                .Setup(repo => repo.GetAll())
                .ReturnsAsync(new List<StockItem> { unlinkedStockItem });

            await _service.Add(command);

            Assert.NotNull(unlinkedStockItem.Definition);
            Assert.Equal(stockItemName, unlinkedStockItem.Definition!.Name.Value);

            _stockItemRepositoryMock.Verify(repo => repo.Update(unlinkedStockItem), Times.Once);
            _productDefinitionRepositoryMock.Verify(repo => repo.Add(It.IsAny<ProductDefinition>()), Times.Once);
        }

        [Fact]
        public async Task Add_ShouldNotLinkStockItem_WhenNameDoesNotMatch()
        {
            var command = new AddProductDefinitionCommand("Mleko waniliowe", UnitType.Liters, Category.Dairy);
            var unrelatedStockItem = new StockItem("Cebula", 1, StorageLocation.Pantry, null);

            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetByName(command.Name))
                .ReturnsAsync((ProductDefinition?)null);

            _stockItemRepositoryMock
                .Setup(repo => repo.GetAll())
                .ReturnsAsync(new List<StockItem> { unrelatedStockItem });

            await _service.Add(command);

            Assert.Null(unrelatedStockItem.Definition);
            _stockItemRepositoryMock.Verify(repo => repo.Update(It.IsAny<StockItem>()), Times.Never);
        }

        [Fact]
        public async Task Add_ShouldNotOverwriteDefinition_WhenStockItemAlreadyHasDefinition()
        {
            var stockItemName = "Mleko waniliowe";
            var command = new AddProductDefinitionCommand(stockItemName, UnitType.Liters, Category.Dairy);

            var existingDefinition = new ProductDefinition(stockItemName, UnitType.Liters, Category.Dairy);
            var alreadyLinkedStockItem = new StockItem(stockItemName, 1, StorageLocation.Fridge, existingDefinition);

            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetByName(stockItemName))
                .ReturnsAsync((ProductDefinition?)null);

            _stockItemRepositoryMock
                .Setup(repo => repo.GetAll())
                .ReturnsAsync(new List<StockItem> { alreadyLinkedStockItem });

            await _service.Add(command);

            Assert.Same(existingDefinition, alreadyLinkedStockItem.Definition);
            _stockItemRepositoryMock.Verify(repo => repo.Update(It.IsAny<StockItem>()), Times.Never);
        }
    }
}
