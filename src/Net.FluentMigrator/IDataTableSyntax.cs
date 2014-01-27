namespace Net.FluentMigrator
{
    public interface IDataTableSyntax
    {
        bool Exists(object row);
    }
}