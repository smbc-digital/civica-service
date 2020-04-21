using civica_service.Utils.Xml;
using System.Xml.Linq;
using Xunit;

namespace civica_service_tests.Utils
{
    public class XmlParserTests
    {
        private readonly XmlParser _xmlParser;
        private const string ElementValue = "test";

        public XmlParserTests()
        {
            _xmlParser = new XmlParser();
        }

        [Fact]
        public void DeserializeXmlStringToType_ReturnsCorrectValue()
        {
            // Act
            var result = _xmlParser.DeserializeXmlStringToType<XmlParserModel>($"<Root><Element>{ElementValue}</Element></Root>", "Root");

            // Assert
            Assert.Equal(ElementValue, result.XmlParserTest);
        }

        [Fact]
        public void DeserializeNodeToType_ReturnsCorrectValue()
        {
            // Act
            var test = _xmlParser.DeserializeNodeToType<XmlParserModel>(XElement.Parse($"<Root><Element>{ElementValue}</Element></Root>"), "Root");

            // Assert
            Assert.Equal(ElementValue, test.XmlParserTest);
        }
    }
}
