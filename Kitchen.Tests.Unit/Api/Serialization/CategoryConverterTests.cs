using System.Text.Encodings.Web;
using System.Text.Json;
using FluentAssertions;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.Domain.Exceptions;
using Xunit;

namespace Kitchen.Tests.Unit.Api.Serialization
{
    public class CategoryConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public CategoryConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new CategoryConverter() },
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        [Theory]
        [InlineData("meat", Category.Meat)]
        [InlineData("mięso", Category.Meat)]
        [InlineData("vegetables", Category.Vegetables)]
        [InlineData("warzywa", Category.Vegetables)]
        [InlineData("dairy", Category.Dairy)]
        [InlineData("nabiał", Category.Dairy)]
        [InlineData("drygoods", Category.DryGoods)]
        [InlineData("sypkie", Category.DryGoods)]
        [InlineData("spices", Category.Spices)]
        [InlineData("przyprawy", Category.Spices)]
        [InlineData("other", Category.Other)]
        [InlineData("inne", Category.Other)]
        [InlineData("MEAT", Category.Meat)]
        public void Read_ShouldMapNameOrDescriptionToCategory(string input, Category expected)
        {
            var json = $"\"{input}\"";

            var result = JsonSerializer.Deserialize<Category>(json, _options);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("-")]
        [InlineData("unspecified")]
        public void Read_ShouldReturnUnspecified_WhenValueMeansUnspecified(string input)
        {
            var result = JsonSerializer.Deserialize<Category>($"\"{input}\"", _options);

            result.Should().Be(Category.Unspecified);
        }

        [Fact]
        public void Read_ShouldThrow_WhenInputIsUnrecognized()
        {
            var action = () => JsonSerializer.Deserialize<Category>("\"banana\"", _options);

            action.Should().Throw<UnknownCategoryException>();
        }

        [Theory]
        [InlineData(Category.Meat, "mięso")]
        [InlineData(Category.Vegetables, "warzywa")]
        [InlineData(Category.Dairy, "nabiał")]
        [InlineData(Category.DryGoods, "sypkie")]
        [InlineData(Category.Spices, "przyprawy")]
        [InlineData(Category.Other, "inne")]
        [InlineData(Category.Unspecified, "-")]
        public void Write_ShouldOutputDescription(Category category, string expectedDescription)
        {
            var json = JsonSerializer.Serialize(category, _options);

            json.Should().Be($"\"{expectedDescription}\"");
        }
    }
}
