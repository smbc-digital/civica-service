using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace civica_service.Helpers.QueryBuilder
{
    public class QueryBuilder : IQueryBuilder
    {
        private readonly List<string> _queryStrings = new List<string>();

        private readonly string _baseUrl;

        public QueryBuilder(IConfiguration configuration)
        {
            _baseUrl = configuration.GetValue("QueryBuilderBaseUrl", string.Empty) + "?outputtype=xml";
        }

        public QueryBuilder Add(string key, string value)
        {
            _queryStrings.Add($"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}");

            return this;
        }

        public string Build()
        {
            var url = _queryStrings.Aggregate($"{_baseUrl}", (previous, current) => $"{previous}&{current}");

            _queryStrings.Clear();

            return url;
        }
    }
}
