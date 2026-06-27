using System.Text.Json;
using FluentAssertions;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;

namespace Kitchen.Tests.Unit.Infrastructure.Middleware;

public class ExceptionMiddlewareTests
{
    private readonly ExceptionMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;

    public ExceptionMiddlewareTests()
    {
        _middleware = new ExceptionMiddleware();
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    private async Task<JsonElement> ReadResponseBody()
    {
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        return await JsonSerializer.DeserializeAsync<JsonElement>(_httpContext.Response.Body);
    }

    #region Happy path

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenNoExceptionOccurs()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };

        // Act
        await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        nextCalled.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    #endregion

    #region 404 Not Found

    [Fact]
    public async Task InvokeAsync_ShouldReturn404_WhenProductDefinitionNotFound()
    {
        // Arrange
        RequestDelegate next = _ => throw new ProductDefinitionNotFoundException();

        // Act
        await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var body = await ReadResponseBody();
        body.GetProperty("code").GetString().Should().Be("product_definition_not_found");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn404_WhenStockItemNotFound()
    {
        // Arrange
        RequestDelegate next = _ => throw new StockItemNotFoundException();

        // Act
        await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var body = await ReadResponseBody();
        body.GetProperty("code").GetString().Should().Be("stock_item_not_found");
    }

    #endregion

    #region 409 Conflict

    [Fact]
    public async Task InvokeAsync_ShouldReturn409_WhenProductDefinitionAlreadyExists()
    {
        // Arrange
        RequestDelegate next = _ => throw new ProductDefinitionAlreadyExistsException();

        // Act
        await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);

        var body = await ReadResponseBody();
        body.GetProperty("code").GetString().Should().Be("product_definition_already_exists");
    }

    #endregion

    #region 400 Bad Request

    [Fact]
    public async Task InvokeAsync_ShouldReturn400_WhenProductNameIsInvalid()
    {
        // Arrange
        RequestDelegate next = _ => throw new InvalidProductNameException();

        // Act
        await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var body = await ReadResponseBody();
        body.GetProperty("code").GetString().Should().Be("invalid_product_name");
    }

    #endregion

    #region 500 Internal Server Error

    [Fact]
    public async Task InvokeAsync_ShouldReturn500_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        RequestDelegate next = _ => throw new Exception("Unexpected error");

        // Act
        await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var body = await ReadResponseBody();
        body.GetProperty("message").GetString().Should().Be("Unexpected error");
    }

    #endregion
}
