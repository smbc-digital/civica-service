using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace civica_service.Helpers.QueryBuilder
{
    public class QueryBuilder : IQueryBuilder
    {
        private readonly List<string> _queryStrings = new List<string>();

        public IQueryBuilder Add(string key, string value)
        {
            _queryStrings.Add($"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}");

            return this;
        }

        public string Build()
        {
            var url = _queryStrings.Aggregate("?outputtype=xml", (previous, current) => $"{previous}&{current}");

            _queryStrings.Clear();

            return url;
        }
    }
}
