namespace civica_service.Helpers.QueryBuilder
{
    public interface IQueryBuilder
    {
        string Build();

        QueryBuilder Add(string key, string value);
    }
}
