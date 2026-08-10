using AwesomeAssertions;
using Kitchen.Application.Commands;
using Kitchen.Api.Requests;
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

        private const string FirstValidName = "Mleko";
        private const string SecondValidName = "Masło";
        private const UnitType ValidUnitType = UnitType.Liters;
        private const Category ValidCategory = Category.Dairy;

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
            var expectedDefinitions = new List<ProductDefinition>
            {
                new(FirstValidName, ValidUnitType, ValidCategory),
                new(SecondValidName, ValidUnitType, ValidCategory)
            };

            _catalogServiceMock
                .Setup(s => s.GetAll())
                .ReturnsAsync(expectedDefinitions);

            // Act
            var response = await _controller.GetAll();

            // Assert
            var result = response.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().BeEquivalentTo(expectedDefinitions);
            _catalogServiceMock.Verify(s => s.GetAll(), Times.Once);
        }

        #endregion

        #region Get

        [Fact]
        public async Task Get_ShouldReturnOk_WhenProductDefinitionExists()
        {
            // Arrange
            var name = FirstValidName;
            var expected = new ProductDefinition(name, ValidUnitType, ValidCategory);

            _catalogServiceMock
                .Setup(s => s.GetByName(name))
                .ReturnsAsync(expected);

            // Act
            var response = await _controller.Get(name);

            // Assert
            var result = response.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().Be(expected);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenProductDefinitionDoesNotExist()
        {
            // Arrange
            _catalogServiceMock
                .Setup(s => s.GetByName(It.IsAny<string>()))
                .ReturnsAsync((ProductDefinition?)null);

            // Act
            var response = await _controller.Get("Nieznany");

            // Assert
            response.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateProductDefinitionRequest(FirstValidName, ValidUnitType, ValidCategory);

            var createdDefinition = new ProductDefinition(request.Name, request.Unit, request.Category);

            _catalogServiceMock
                .Setup(s => s.Add(It.IsAny<AddProductDefinitionCommand>()))
                .ReturnsAsync(createdDefinition);

            // Act
            var response = await _controller.Create(request);

            // Assert
            var result = response.Should().BeOfType<CreatedAtActionResult>().Subject;

            result.ActionName.Should().Be("Get");
            result.RouteValues.Should()
                    .ContainKey("name")
                    .WhoseValue.Should().Be(createdDefinition.Name.Value);

            result.Value.Should().Be(createdDefinition);

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
            var request = new UpdateProductDefinitionRequest(Unit: ValidUnitType);

            // Act
            var response = await _controller.Update(FirstValidName, request);

            // Assert
            response.Should().BeOfType<NoContentResult>();

            _catalogServiceMock.Verify(s => s.Update(It.Is<ModifyProductDefinitionCommand>(c =>
                c.Name == FirstValidName &&
                c.Unit == request.Unit)), Times.Once);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
        {
            // Act
            var response = await _controller.Delete(FirstValidName);

            // Assert
            response.Should().BeOfType<NoContentResult>();
            _catalogServiceMock.Verify(s => s.Delete(FirstValidName), Times.Once);
        }

        #endregion
    }
}