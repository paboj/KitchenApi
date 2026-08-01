using System.Text.Json;
using FluentAssertions;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.ValueObjects;
using Xunit;

namespace Kitchen.Tests.Unit.Api.Serialization
{
    public class ProductNameConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public ProductNameConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new ProductNameConverter() }
            };
        }

        [Fact]
        public void Read_ShouldWrapStringInProductName()
        {
            var result = JsonSerializer.Deserialize<ProductName>("\"Mleko\"", _options);

            result.Should().NotBeNull();
            result!.Value.Should().Be("Mleko");
        }

        [Fact]
        public void Read_ShouldThrow_WhenValueIsBlank()
        {
            var action = () => JsonSerializer.Deserialize<ProductName>("\"   \"", _options);

            action.Should().Throw<InvalidProductNameException>();
        }

        [Fact]
        public void Write_ShouldOutputPlainString()
        {
            var productName = new ProductName("Mleko");

            var json = JsonSerializer.Serialize(productName, _options);

            json.Should().Be("\"Mleko\"");
        }
    }
}
