namespace civica_service.Helpers.QueryBuilder
{
    public interface IQueryBuilder
    {
        string Build();

        IQueryBuilder Add(string key, string value);
    }
}
