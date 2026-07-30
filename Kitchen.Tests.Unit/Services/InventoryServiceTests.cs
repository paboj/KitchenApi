using FluentAssertions;
using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.Repositories;
using Moq;

namespace Kitchen.Tests.Unit.Services
{
    public class InventoryServiceTests
    {
        private readonly Mock<IStockItemRepository> _stockItemRepositoryMock;
        private readonly Mock<IProductDefinitionRepository> _productDefinitionRepositoryMock;
        private readonly InventoryService _service;

        public InventoryServiceTests()
        {
            _stockItemRepositoryMock = new Mock<IStockItemRepository>();
            _productDefinitionRepositoryMock = new Mock<IProductDefinitionRepository>();
            _service = new InventoryService(_stockItemRepositoryMock.Object, _productDefinitionRepositoryMock.Object);
        }

        [Fact]
        public async Task GetByName_ShouldReturnStockItems_WhenStockItemsExist()
        {
            // Arrange
            var stockItemName = "Onion";
            var firstExpectedStockItem = new StockItem(stockItemName, 10, StorageLocation.Fridge, null);
            var secondExpectedStockItem = new StockItem(stockItemName, 5, StorageLocation.Freezer, null);
            IEnumerable<StockItem> expectedStockItems = new List<StockItem> { firstExpectedStockItem, secondExpectedStockItem };

            _stockItemRepositoryMock
                .Setup(repo => repo.GetByNameWithDetails(stockItemName))
                .ReturnsAsync(expectedStockItems);

            // Act
            var result = await _service.GetByName(stockItemName);

            // Assert
            result.Should().NotBeNull();
            result.Should().OnlyContain(item => item.Name.Value == stockItemName);

            // Optional
            _stockItemRepositoryMock.Verify(repo => repo.GetByNameWithDetails(stockItemName), Times.Once);
        }

        [Fact]
        public async Task GetByName_ShouldReturnNull_WhenStockItemsDoNotExist()
        {
            // Arrange
            _stockItemRepositoryMock
                .Setup(repo => repo.GetByNameWithDetails(It.IsAny<string>()))
                .ReturnsAsync(Enumerable.Empty<StockItem>());

            // Act
            var result = await _service.GetByName("NonExistent");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ShouldReturnStockItems_WhenStockItemsExists()
        {
            // Arrange
            IEnumerable<StockItem> expectedStockItems = new List<StockItem>
            {
                new StockItem("Onion", 10, StorageLocation.Fridge, null),
                new StockItem("Potato", 5, StorageLocation.Pantry, null)
            };

            _stockItemRepositoryMock
                .Setup(repo => repo.GetAllWithDetails())
                .ReturnsAsync(expectedStockItems);

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().Equal(expectedStockItems);

            _stockItemRepositoryMock.Verify(repo => repo.GetAllWithDetails(), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnStockItem_WhenItExists()
        {
            var stockItem = new StockItem("Onion", 10, StorageLocation.Fridge, null);

            _stockItemRepositoryMock
                .Setup(repo => repo.GetByIdWithDetails(stockItem.Id.Value))
                .ReturnsAsync(stockItem);

            var result = await _service.GetById(stockItem.Id.Value);

            result.Should().Be(stockItem);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenItDoesNotExist()
        {
            var id = Guid.NewGuid();

            _stockItemRepositoryMock
                .Setup(repo => repo.GetByIdWithDetails(id))
                .ReturnsAsync((StockItem?)null);

            var result = await _service.GetById(id);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Add_ShouldSucceed_WhenValidStockItem()
        {
            var command = new AddStockItemCommand("Onion", 10, StorageLocation.Fridge);

            await _service.Add(command);

            _stockItemRepositoryMock.Verify(repo => repo.Add(It.Is<StockItem>(i =>
                i.Name == command.Name &&
                i.Amount == command.Amount &&
                i.Location == command.Location)),
                Times.Once);
        }

        [Fact]
        public async Task Add_ShouldLinkExistingProductDefinition_WhenOneMatchesByName()
        {
            var name = "Onion";
            var command = new AddStockItemCommand(name, 10, StorageLocation.Fridge);
            var existingDefinition = new ProductDefinition(name, UnitType.Kilograms, Category.Vegetables);

            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetByName(name))
                .ReturnsAsync(existingDefinition);

            await _service.Add(command);

            _stockItemRepositoryMock.Verify(repo => repo.Add(It.Is<StockItem>(i =>
                i.Definition == existingDefinition)),
                Times.Once);
        }

        [Fact]
        public async Task Update_ShouldSucceed_WhenValidStockItem()
        {
            var StockItemName = "Onion";
            var initialAmount = 10;
            var newAmount = 5;
            var initialLocation = StorageLocation.Fridge;
            var newLocation = StorageLocation.Pantry;

            var existingStockItem = new StockItem(StockItemName, initialAmount, initialLocation, null);

            var existingStockItemId = existingStockItem.Id.Value;

            _stockItemRepositoryMock
                .Setup(repo => repo.GetByIdWithDetails(existingStockItemId))
                .ReturnsAsync(existingStockItem);

            var command = new ModifyStockItemCommand(existingStockItemId, StockItemName, newAmount, newLocation);

            await _service.Update(command);

            existingStockItem.Amount.Should().Be(newAmount);
            existingStockItem.Location.Should().Be(newLocation);

            _stockItemRepositoryMock.Verify(repo => repo.Update(existingStockItem), Times.Once);

        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenStockItemDoesNotExist()
        {
            var id = Guid.NewGuid();

            _stockItemRepositoryMock
                .Setup(repo => repo.GetByIdWithDetails(id))
                .ReturnsAsync((StockItem?)null);

            var command = new ModifyStockItemCommand(id, "Onion", 5, StorageLocation.Pantry);

            var action = async () => await _service.Update(command);

            await action.Should().ThrowAsync<StockItemNotFoundException>();
            _stockItemRepositoryMock.Verify(repo => repo.Update(It.IsAny<StockItem>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ShouldCallRepository_WhenStockItemExists()
        {
            var StockItemName = "Onion";
            var existingStockItem = new StockItem(StockItemName, 10, StorageLocation.Fridge, null);
            var existingStockItemId = existingStockItem.Id.Value;

            _stockItemRepositoryMock
                .Setup(repo => repo.GetByIdWithDetails(existingStockItemId))
                .ReturnsAsync(existingStockItem);

            await _service.Delete(existingStockItemId);

            _stockItemRepositoryMock.Verify(repo => repo.Delete(existingStockItemId), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrowException_WhenStockItemDoesNotExists()
        {
            var StockItemId = new Guid();

            _stockItemRepositoryMock
                .Setup(repo => repo.GetByIdWithDetails(StockItemId))
                .ReturnsAsync((StockItem?)null);

            var action = async () => await _service.Delete(StockItemId);

            await action.Should().ThrowAsync<StockItemNotFoundException>();

            _stockItemRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Never);
        }
    }
}
