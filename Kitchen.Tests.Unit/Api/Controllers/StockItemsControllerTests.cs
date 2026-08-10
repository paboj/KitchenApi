using AwesomeAssertions;
using Kitchen.Application.Commands;
using Kitchen.Api.Requests;
using Kitchen.Application.Services;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Kitchen.Tests.Unit.Api.Controllers
{
    public class StockItemsControllerTests
    {
        private const string ValidName = "Pomidor";
        private const string ValidName2 = "Cebula";
        private const int ValidAmount = 5;
        private const StorageLocation ValidLocation = StorageLocation.Fridge;
        private const int ValidExpiringDays = 7;

        private readonly Mock<IInventoryService> _inventoryServiceMock;
        private readonly StockItemsController _controller;

        public StockItemsControllerTests()
        {
            _inventoryServiceMock = new Mock<IInventoryService>();
            _controller = new StockItemsController(_inventoryServiceMock.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateStockItemRequest(ValidName, ValidAmount, ValidLocation);

            var createdStockItem = new StockItem(request.Name, request.Amount, request.Location, null);

            _inventoryServiceMock
                .Setup(s => s.Add(It.IsAny<AddStockItemCommand>()))
                .ReturnsAsync(createdStockItem);

            // Act
            var response = await _controller.Create(request);

            // Assert
            var result = response.Should().BeOfType<CreatedAtActionResult>().Subject;

            result.ActionName.Should().Be("Get");
            result.RouteValues!["id"].Should().Be(createdStockItem.Id.Value);
            result.Value.Should().Be(createdStockItem);

            _inventoryServiceMock.Verify(s => s.Add(It.Is<AddStockItemCommand>(c =>
                c.Name == request.Name &&
                c.Amount == request.Amount &&
                c.Location == request.Location)), Times.Once);
        }

        #region GetAll

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithStockItems()
        {
            var expected = new List<StockItem>
            {
                new(ValidName, ValidAmount, ValidLocation, null),
                new(ValidName2, ValidAmount, ValidLocation, null)
            };

            _inventoryServiceMock
                .Setup(s => s.GetAll())
                .ReturnsAsync(expected);

            var response = await _controller.GetAll();

            var result = response.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().BeEquivalentTo(expected);
            _inventoryServiceMock.Verify(s => s.GetAll(), Times.Once);
        }

        #endregion

        #region Get by id

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenStockItemExists()
        {
            var stockItem = new StockItem(ValidName, ValidAmount, ValidLocation, null);

            _inventoryServiceMock
                .Setup(s => s.GetById(stockItem.Id.Value))
                .ReturnsAsync(stockItem);

            var response = await _controller.Get(stockItem.Id.Value);

            var result = response.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().Be(stockItem);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenStockItemDoesNotExist()
        {
            _inventoryServiceMock
                .Setup(s => s.GetById(It.IsAny<Guid>()))
                .ReturnsAsync((StockItem?)null);

            var response = await _controller.Get(Guid.NewGuid());

            response.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region Get by name

        [Fact]
        public async Task GetByName_ShouldReturnOk_WhenStockItemsExist()
        {
            var name = "Ziemniak";
            var expected = new List<StockItem> { new(name, ValidAmount, ValidLocation, null) };

            _inventoryServiceMock
                .Setup(s => s.GetByName(name))
                .ReturnsAsync(expected);

            var response = await _controller.Get(name);

            var result = response.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByName_ShouldReturnNotFound_WhenNoStockItemsMatchName()
        {
            _inventoryServiceMock
                .Setup(s => s.GetByName(It.IsAny<string>()))
                .ReturnsAsync(Enumerable.Empty<StockItem>());

            var response = await _controller.Get("Nieznany");

            response.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region GetExpiring

        [Fact]
        public async Task GetExpiring_ShouldReturnOk_WithStockItems()
        {
            var expected = new List<StockItem> { new(ValidName, ValidAmount, ValidLocation, null) };

            _inventoryServiceMock
                .Setup(s => s.GetExpiring(ValidExpiringDays))
                .ReturnsAsync(expected);

            var response = await _controller.GetExpiring(ValidExpiringDays);

            var result = response.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().BeEquivalentTo(expected);
            _inventoryServiceMock.Verify(s => s.GetExpiring(ValidExpiringDays), Times.Once);
        }

        [Fact]
        public async Task GetExpiring_ShouldDefaultToSevenDays_WhenDaysNotProvided()
        {
            _inventoryServiceMock
                .Setup(s => s.GetExpiring(It.IsAny<int>()))
                .ReturnsAsync(Enumerable.Empty<StockItem>());

            await _controller.GetExpiring();

            _inventoryServiceMock.Verify(s => s.GetExpiring(ValidExpiringDays), Times.Once);
        }

        [Fact]
        public async Task GetExpiring_ShouldReturnOk_WithEmptyList_WhenNoneAreExpiring()
        {
            _inventoryServiceMock
                .Setup(s => s.GetExpiring(It.IsAny<int>()))
                .ReturnsAsync(Enumerable.Empty<StockItem>());

            var response = await _controller.GetExpiring(ValidExpiringDays);

            var result = response.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().BeEquivalentTo(Enumerable.Empty<StockItem>());
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenValid()
        {
            var id = Guid.NewGuid();
            var request = new UpdateStockItemRequest(Amount: ValidAmount, Location: ValidLocation);

            var response = await _controller.Update(id, request);

            response.Should().BeOfType<NoContentResult>();

            _inventoryServiceMock.Verify(s => s.Update(It.Is<ModifyStockItemCommand>(c =>
                c.Id == id &&
                c.Amount == request.Amount &&
                c.Location == request.Location)), Times.Once);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();

            var response = await _controller.Delete(id);

            response.Should().BeOfType<NoContentResult>();
            _inventoryServiceMock.Verify(s => s.Delete(id), Times.Once);
        }

        #endregion
    }
}