using System.Web;
using civica_service.Helpers.QueryBuilder;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace civica_service_tests.Helpers
{
    public class QueryBuilderTests
    {
        private readonly IQueryBuilder _queryBuild;
        private const string TestUrl = "http://test.gov.uk";

        public QueryBuilderTests()
        {
            var mockConfigurationSection = new Mock<IConfigurationSection>();
            mockConfigurationSection
                .Setup(_ => _.Value)
                .Returns(TestUrl);

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration
                .Setup(_ => _.GetSection("QueryBuilderBaseUrl"))
                .Returns(mockConfigurationSection.Object);

            _queryBuild = new QueryBuilder(mockConfiguration.Object);
        }

        [Theory]
        [InlineData("test1", "value1")]
        [InlineData("", "")]
        [InlineData("%^£", "&£$")]
        [InlineData(" ", " ")]
        public void Add_ShouldAddEncodedStringToUrl(string key, string value)
        {
            var expectedUrl = $"{TestUrl}?outputtype=xml&{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}";

            var response = _queryBuild.Add(key, value).Build();

            Assert.Equal(expectedUrl, response);
        }

        [Fact]
        public void Add_ShouldAddEncodedStringsToUrl()
        {
            var expectedUrl = $"{TestUrl}?outputtype=xml&{HttpUtility.UrlEncode("test")}={HttpUtility.UrlEncode("test")}&{HttpUtility.UrlEncode("test1")}={HttpUtility.UrlEncode("test1")}";

            var response = _queryBuild
                .Add("test", "test")
                .Add("test1", "test1")
                .Build();

            Assert.Equal(expectedUrl, response);
        }
    }
}