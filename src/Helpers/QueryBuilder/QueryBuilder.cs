using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace civica_service.Helpers.QueryBuilder
{
    public class QueryBuilder : IQueryBuilder
    {
        private List<string> queryStrings = new List<string>();

        public QueryBuilder Add(string key, string value)
        {
            queryStrings.Add($"{HttpUtility.UrlEncode(key)}=\"{HttpUtility.UrlEncode(value)}\"");

            return this;
        }

        public string Build()
        {
            var baseQuery = "http://www.google.com/exampleScript.php";

            var query = queryStrings.Aggregate(
                $"{baseQuery}?",
                (previous, current) =>
                {
                    return $"{previous}&{current}";
                });

            return query;
        }
    }
}
