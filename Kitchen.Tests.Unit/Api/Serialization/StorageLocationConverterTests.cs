using System.Text.Encodings.Web;
using System.Text.Json;
using FluentAssertions;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Xunit;

namespace Kitchen.Tests.Unit.Api.Serialization
{
    public class StorageLocationConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public StorageLocationConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new StorageLocationConverter() },
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        [Theory]
        [InlineData("fridge", StorageLocation.Fridge)]
        [InlineData("lodówka", StorageLocation.Fridge)]
        [InlineData("freezer", StorageLocation.Freezer)]
        [InlineData("zamrażarka", StorageLocation.Freezer)]
        [InlineData("pantry", StorageLocation.Pantry)]
        [InlineData("szafki", StorageLocation.Pantry)]
        [InlineData("FRIDGE", StorageLocation.Fridge)]
        public void Read_ShouldMapNameOrDescriptionToStorageLocation(string input, StorageLocation expected)
        {
            var json = $"\"{input}\"";

            var result = JsonSerializer.Deserialize<StorageLocation>(json, _options);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("-")]
        [InlineData("unspecified")]
        public void Read_ShouldReturnUnspecified_WhenValueMeansUnspecified(string input)
        {
            var result = JsonSerializer.Deserialize<StorageLocation>($"\"{input}\"", _options);

            result.Should().Be(StorageLocation.Unspecified);
        }

        [Fact]
        public void Read_ShouldThrow_WhenInputIsUnrecognized()
        {
            var action = () => JsonSerializer.Deserialize<StorageLocation>("\"banana\"", _options);

            action.Should().Throw<UnknownLocationException>();
        }

        [Theory]
        [InlineData(StorageLocation.Fridge, "lodówka")]
        [InlineData(StorageLocation.Freezer, "zamrażarka")]
        [InlineData(StorageLocation.Pantry, "szafki")]
        [InlineData(StorageLocation.Unspecified, "-")]
        public void Write_ShouldOutputDescription(StorageLocation location, string expectedDescription)
        {
            var json = JsonSerializer.Serialize(location, _options);

            json.Should().Be($"\"{expectedDescription}\"");
        }
    }
}
