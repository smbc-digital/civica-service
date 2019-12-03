using civica_service.Utils.Xml;
using System.Xml.Linq;
using Xunit;

namespace civica_service_tests.Utils
{
    public class XmlParserTests
    {
        private readonly XmlParser _xmlParser;
        private const string elementValue = "test";

        public XmlParserTests()
        {
            _xmlParser = new XmlParser();
        }

        [Fact]
        public void DeserializeXmlStringToType_ReturnsCorrectValue()
        {
            // Act
            var result = _xmlParser.DeserializeXmlStringToType<XmlParserModel>($"<Root><Element>{elementValue}</Element></Root>", "Root");

            // Assert
            Assert.Equal(elementValue, result.XmlParserTest);
        }

        [Fact]
        public void DeserializeNodeToType_ReturnsCorrectValue()
        {
            // Act
            var test = _xmlParser.DeserializeNodeToType<XmlParserModel>(XElement.Parse($"<Root><Element>{elementValue}</Element></Root>"), "Root");

            // Assert
            Assert.Equal(elementValue, test.XmlParserTest);
        }
    }
}
