using AwesomeAssertions;
using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.Repositories;
using Kitchen.Core.ValueObjects;
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

            await action.Should().ThrowAsync<ProductDefinitionAlreadyExistsException>();

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

            unlinkedStockItem.Definition.Should().NotBeNull();
            unlinkedStockItem.Definition!.Name.Value.Should().Be(new ProductName(stockItemName));

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

            unrelatedStockItem.Definition.Should().BeNull();
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

            alreadyLinkedStockItem.Definition.Should().BeSameAs(existingDefinition);
            _stockItemRepositoryMock.Verify(repo => repo.Update(It.IsAny<StockItem>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllProductDefinitions()
        {
            var expected = new List<ProductDefinition>
            {
                new("Mąka", UnitType.Kilograms, Category.DryGoods),
                new("Mleko", UnitType.Liters, Category.Dairy)
            };

            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetAll())
                .ReturnsAsync(expected);

            var result = await _service.GetAll();

            result.Should().Equal(expected);
        }

        [Fact]
        public async Task GetByName_ShouldReturnNull_WhenProductDefinitionDoesNotExist()
        {
            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetByName(It.IsAny<string>()))
                .ReturnsAsync((ProductDefinition?)null);

            var result = await _service.GetByName("Nieznany");

            result.Should().BeNull();
        }

        [Fact]
        public async Task Update_ShouldApplyChanges_WhenProductDefinitionExists()
        {
            var name = "Mleko waniliowe";
            var existingDefinition = new ProductDefinition(name, UnitType.Liters, Category.Dairy);

            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetByName(name))
                .ReturnsAsync(existingDefinition);

            var command = new ModifyProductDefinitionCommand(name, UnitType.Kilograms, Category.DryGoods);

            await _service.Update(command);

            existingDefinition.Unit.Should().Be(UnitType.Kilograms);
            existingDefinition.Category.Should().Be(Category.DryGoods);
            _productDefinitionRepositoryMock.Verify(repo => repo.Update(existingDefinition), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenProductDefinitionDoesNotExist()
        {
            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetByName(It.IsAny<string>()))
                .ReturnsAsync((ProductDefinition?)null);

            var command = new ModifyProductDefinitionCommand("Nieznany", UnitType.Kilograms, Category.DryGoods);

            var action = async () => await _service.Update(command);

            await action.Should().ThrowAsync<ProductDefinitionNotFoundException>();
            _productDefinitionRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductDefinition>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ShouldCallRepository_WhenProductDefinitionExists()
        {
            var name = "Mleko waniliowe";

            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetByName(name))
                .ReturnsAsync(new ProductDefinition(name, UnitType.Liters, Category.Dairy));

            await _service.Delete(name);

            _productDefinitionRepositoryMock.Verify(repo => repo.Delete(name), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenProductDefinitionDoesNotExist()
        {
            _productDefinitionRepositoryMock
                .Setup(repo => repo.GetByName(It.IsAny<string>()))
                .ReturnsAsync((ProductDefinition?)null);

            var action = async () => await _service.Delete("Nieznany");

            await action.Should().ThrowAsync<ProductDefinitionNotFoundException>();
            _productDefinitionRepositoryMock.Verify(repo => repo.Delete(It.IsAny<string>()), Times.Never);
        }
    }
}
