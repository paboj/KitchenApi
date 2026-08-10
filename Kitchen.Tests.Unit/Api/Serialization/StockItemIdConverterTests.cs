using System.Text.Json;
using AwesomeAssertions;
using Kitchen.Core.ValueObjects;
using Xunit;

namespace Kitchen.Tests.Unit.Api.Serialization
{
    public class StockItemIdConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public StockItemIdConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new StockItemIdConverter() }
            };
        }

        [Fact]
        public void Read_ShouldWrapGuidInStockItemId()
        {
            var guid = Guid.NewGuid();
            var json = $"\"{guid}\"";

            var result = JsonSerializer.Deserialize<StockItemId>(json, _options);

            result.Should().Be(new StockItemId(guid));
        }

        [Fact]
        public void Read_ShouldThrow_WhenValueIsNotAValidGuid()
        {
            var action = () => JsonSerializer.Deserialize<StockItemId>("\"not-a-guid\"", _options);

            action.Should().Throw<JsonException>()
                .WithInnerException<FormatException>();
        }

        [Fact]
        public void Write_ShouldOutputPlainGuidString()
        {
            var guid = Guid.NewGuid();
            var stockItemId = new StockItemId(guid);

            var json = JsonSerializer.Serialize(stockItemId, _options);

            json.Should().Be($"\"{guid}\"");
        }
    }
}
