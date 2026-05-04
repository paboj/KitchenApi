using FluentAssertions;
using Kitchen.Application.Commands;
using Kitchen.Application.Models.Requests;
using Kitchen.Application.Services;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Kitchen.Tests.Unit.Api.Controllers
{
    public class IngredientTypesControllerTests
    {
        #region Arrange

        private readonly Mock<ICatalogService> _catalogServiceMock;
        private readonly IngredientTypesController _controller;

        public IngredientTypesControllerTests()
        {
            _catalogServiceMock = new Mock<ICatalogService>();
            _controller = new IngredientTypesController(_catalogServiceMock.Object);
        }

        #endregion

        #region GetAll

        [Fact]
        public void get_all_should_return_ok_with_ingredient_types()
        {
            // Arrange
            var expectedTypes = new List<IngredientType>
            {
                new("Mąka", UnitType.Grams),
                new("Mleko", UnitType.Milliliters)
            };

            _catalogServiceMock
                .Setup(s => s.GetAll())
                .Returns(expectedTypes);

            // Act
            var response = _controller.GetAll();

            // Assert
            var result = response.Should().BeOfType<OkObjectResult>().Subject;
            result.Value.Should().BeEquivalentTo(expectedTypes);
            _catalogServiceMock.Verify(s => s.GetAll(), Times.Once);
        }

        #endregion

        #region Create

        [Fact]
        public void create_should_return_created_at_action_when_request_is_valid()
        {
            // Arrange
            var request = new CreateIngredientTypeRequest
            {
                Name = "Cukier",
                Unit = UnitType.Grams
            };

            // Act
            var response = _controller.Create(request);

            // Assert
            var result = response.Should().BeOfType<CreatedAtActionResult>().Subject;

            result.ActionName.Should().Be("GetAll");
            result.RouteValues["name"].Should().Be(request.Name);

            var command = result.Value.Should().BeOfType<AddTypeCatalogCommand>().Subject;
            command.Name.Should().Be(request.Name);
            command.Unit.Should().Be(request.Unit);

            _catalogServiceMock.Verify(s => s.Add(It.Is<AddTypeCatalogCommand>(c =>
                c.Name == request.Name &&
                c.Unit == request.Unit)), Times.Once);
        }

        #endregion

        #region Update

        [Fact]
        public void update_should_return_no_content_when_valid()
        {
            // Arrange
            var typeName = "Mleko";
            var request = new UpdateIngredientTypeRequest
            {
                Unit = UnitType.Milliliters
            };

            // Act
            var response = _controller.Update(typeName, request);

            // Assert
            response.Should().BeOfType<NoContentResult>();

            _catalogServiceMock.Verify(s => s.Update(It.Is<ModifyTypeCatalogCommand>(c =>
                c.Name == typeName &&
                c.Unit == request.Unit)), Times.Once);
        }

        #endregion

        #region Delete

        [Fact]
        public void delete_should_return_no_content_when_successful()
        {
            // Arrange
            var typeName = "Mąka";

            // Act
            var response = _controller.Delete(typeName);

            // Assert
            response.Should().BeOfType<NoContentResult>();
            _catalogServiceMock.Verify(s => s.Delete(typeName), Times.Once);
        }

        #endregion
    }
}