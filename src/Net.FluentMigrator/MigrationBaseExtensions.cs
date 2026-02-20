using System.Reflection;
using FluentMigrator;
using FluentMigrator.Infrastructure;

namespace Net.FluentMigrator
{
    public static class MigrationBaseExtensions
    {
        static readonly FieldInfo contextField = typeof(MigrationBase).GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance);

        public static IMigrationContext GetMigrationContext(this MigrationBase migrationBase)
        {
            return (IMigrationContext)contextField.GetValue(migrationBase);
        }

        public static IDataSyntax GetData(this MigrationBase migrationBase)
        {
            return new DataExpressionRoot(migrationBase.GetMigrationContext());
        }

        public static IDataSyntax GetData(this IMigrationContext migrationContext)
        {
            return new DataExpressionRoot(migrationContext);
        }
    }
}