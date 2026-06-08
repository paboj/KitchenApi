using FluentAssertions;
using Kitchen.Application.Commands;
using Kitchen.Application.Models.Requests;
using Kitchen.Application.Services;
using Kitchen.Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Kitchen.Tests.Unit.Api.Controllers
{
    public class StockItemsControllerTests
    {
        private readonly Mock<IInventoryService> _inventoryServiceMock;
        private readonly StockItemsController _controller;

        public StockItemsControllerTests()
        {
            _inventoryServiceMock = new Mock<IInventoryService>();
            _controller = new StockItemsController(_inventoryServiceMock.Object);
        }

        [Fact]
        public void create_should_return_created_at_action_when_request_is_valid()
        {
            // Arrange
            var request = new CreateStockItemRequest
            {
                Name = "Tomato",
                Amount = 5,
                Location = StorageLocation.Fridge
            };

            // Act
            var response = _controller.Create(request);

            // Assert
            var result = response.Should().BeOfType<CreatedAtActionResult>().Subject;

            result.ActionName.Should().Be("Get");
            result.RouteValues!["id"].Should().NotBeNull();
            result.Value.Should().Be(request);

            _inventoryServiceMock.Verify(s => s.Add(It.Is<AddStockItemCommand>(c =>
                c.Name == request.Name &&
                c.Amount == request.Amount &&
                c.Location == request.Location)), Times.Once);
        }
    }
}