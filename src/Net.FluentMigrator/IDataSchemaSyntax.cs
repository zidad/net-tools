namespace Net.FluentMigrator
{
    public interface IDataSchemaSyntax
    {
        IDataTableSyntax InTable(string tableName);
    }
}