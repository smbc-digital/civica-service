using civica_service.Utils.Xml;
using Xunit;

namespace civica_service_tests.Utils
{
    public class XmlParserTests
    {
        private readonly XmlParser _xmlParser;

        public XmlParserTests()
        {
            _xmlParser = new XmlParser();
        }

        [Fact]
        public async void DeserializeXmlStringToType()
        {
            // Act
            var result = _xmlParser.DeserializeXmlStringToType<XmlParserModel>("<Root><Element>test</Element></Root>", "Root");

            // Assert
            Assert.Equal("test", "test");
        }

        [Fact]
        public async void DeserializeNodeToType ()
        {
            // Arrange


            // Act


            // Assert
        }
    }
}
