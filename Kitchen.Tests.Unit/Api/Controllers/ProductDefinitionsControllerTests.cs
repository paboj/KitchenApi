using FluentAssertions;
using Kitchen.Application.Commands;
using Kitchen.Application.Models.Requests;
using Kitchen.Application.Services;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kitchen.Tests.Unit.Api.Controllers
{
    public class ProductDefinitionsControllerTests
    {
        #region Arrange

        private readonly Mock<ICatalogService> _catalogServiceMock;
        private readonly ProductDefinitionsController _controller;

        public ProductDefinitionsControllerTests()
        {
            _catalogServiceMock = new Mock<ICatalogService>();
            _controller = new ProductDefinitionsController(_catalogServiceMock.Object);
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithIngredientTypes()
        {
            // Arrange
            var expectedTypes = new List<ProductDefinition>
            {
                new("Mąka", UnitType.Kilograms, Category.DryGoods),
                new("Mleko", UnitType.Liters, Category.Dairy)
            };

            _catalogServiceMock
                .Setup(s => s.GetAll())
                .ReturnsAsync(expectedTypes);

            // Act
            var response = await _controller.GetAll();

            // Assert
            var result = response.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().BeEquivalentTo(expectedTypes);
            _catalogServiceMock.Verify(s => s.GetAll(), Times.Once);
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateProductDefinitionRequest
            {
                Name = "Cukier",
                Unit = UnitType.Kilograms
            };

            // Act
            var response = await _controller.Create(request);

            // Assert
            var result = response.Should().BeOfType<CreatedAtActionResult>().Subject;

            result.ActionName.Should().Be("GetAll");
            //result.RouteValues!["name"].Should().Be(request.Name); - also possible
            result.RouteValues.Should()
                    .ContainKey("name")
                    .WhoseValue.Should().Be(request.Name);

            var command = result.Value.Should().BeOfType<AddProductDefinitionCommand>().Subject;
            command.Name.Should().Be(request.Name);
            command.Unit.Should().Be(request.Unit);

            _catalogServiceMock.Verify(s => s.Add(It.Is<AddProductDefinitionCommand>(c =>
                c.Name == request.Name &&
                c.Unit == request.Unit)), Times.Once);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            var typeName = "Mleko";
            var request = new UpdateProductDefinitionRequest
            {
                Unit = UnitType.Liters
            };

            // Act
            var response = await _controller.Update(typeName, request);

            // Assert
            response.Should().BeOfType<NoContentResult>();

            _catalogServiceMock.Verify(s => s.Update(It.Is<ModifyProductDefinitionCommand>(c =>
                c.Name == typeName &&
                c.Unit == request.Unit)), Times.Once);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var typeName = "Mąka";

            // Act
            var response = await _controller.Delete(typeName);

            // Assert
            response.Should().BeOfType<NoContentResult>();
            _catalogServiceMock.Verify(s => s.Delete(typeName), Times.Once);
        }

        #endregion
    }
}