namespace Net.FluentMigrator
{
    public interface IDataSyntax
    {
        IDataSchemaSyntax InSchema(string schemaName);
        IDataTableSyntax InTable(string tableName);
    }
}