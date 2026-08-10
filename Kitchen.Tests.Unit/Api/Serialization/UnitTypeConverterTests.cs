using System.Text.Json;
using AwesomeAssertions;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Xunit;

namespace Kitchen.Tests.Unit.Api.Serialization
{
    public class UnitTypeConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public UnitTypeConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new UnitTypeConverter() }
            };
        }

        [Theory]
        [InlineData("szt", UnitType.Pieces)]
        [InlineData("sztuk", UnitType.Pieces)]
        [InlineData("pieces", UnitType.Pieces)]
        [InlineData("kg", UnitType.Kilograms)]
        [InlineData("kilograms", UnitType.Kilograms)]
        [InlineData("l", UnitType.Liters)]
        [InlineData("liters", UnitType.Liters)]
        [InlineData("litry", UnitType.Liters)]
        [InlineData("KG", UnitType.Kilograms)]
        public void Read_ShouldMapAliasToUnitType(string alias, UnitType expected)
        {
            var json = $"\"{alias}\"";

            var result = JsonSerializer.Deserialize<UnitType>(json, _options);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("-")]
        [InlineData("unspecified")]
        public void Read_ShouldReturnUnspecified_WhenValueMeansUnspecified(string input)
        {
            var result = JsonSerializer.Deserialize<UnitType>($"\"{input}\"", _options);

            result.Should().Be(UnitType.Unspecified);
        }

        [Fact]
        public void Read_ShouldThrow_WhenAliasIsUnrecognized()
        {
            var action = () => JsonSerializer.Deserialize<UnitType>("\"banana\"", _options);

            action.Should().Throw<UnknownUnitTypeException>();
        }

        [Theory]
        [InlineData(UnitType.Pieces, "szt")]
        [InlineData(UnitType.Kilograms, "kg")]
        [InlineData(UnitType.Liters, "l")]
        [InlineData(UnitType.Unspecified, "-")]
        public void Write_ShouldOutputDescription(UnitType unit, string expectedDescription)
        {
            var json = JsonSerializer.Serialize(unit, _options);

            json.Should().Be($"\"{expectedDescription}\"");
        }
    }
}
