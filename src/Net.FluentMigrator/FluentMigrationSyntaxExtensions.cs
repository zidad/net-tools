using System.Linq;
using FluentMigrator.Builders.Alter;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Column;
using FluentMigrator.Builders.Create.Index;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Delete.Index;
using FluentMigrator.Builders.Schema;
using FluentMigrator.SqlServer;

namespace Net.FluentMigrator
{
    public static class FluentMigrationSyntaxExtensions
    {
        public static ICreateColumnAsTypeSyntax OnTable(this ICreateColumnOnTableSyntax syntax, string schema, string table)
        {
            return syntax.OnTable(table).InSchema(schema);
        }

        public static IAlterTableAddColumnOrAlterColumnSyntax Table(this IAlterExpressionRoot syntax, string schema, string table)
        {
            return syntax.Table(table).InSchema(schema);
        }
        
        public static ICreateColumnAsTypeSyntax Column(this ICreateExpressionRoot syntax, string schema, string table, string column)
        {
            return syntax.Column(column).OnTable(table).InSchema(schema);
        }

        public static IDeleteIndexOnColumnSyntax Index(this IDeleteExpressionRoot syntax, string schema, string table, string index)
        {
            return syntax.Index(index).OnTable(table).InSchema(schema);
        }

        public static void Table(this IDeleteExpressionRoot syntax, string schema, string table)
        {
            syntax.Table(table).InSchema(schema);
        }

        public static ICreateIndexOnColumnSyntax Index(this ICreateExpressionRoot syntax, string schema, string table, string index)
        {
            return syntax.Index(index).OnTable(table).InSchema(schema);
        }

        public static bool Exists(this ISchemaExpressionRoot syntax, string schema, string table)
        {
            return syntax.Schema(schema).Table(table).Exists();
        }

        public static ICreateIndexOnColumnSyntax Index(this ICreateExpressionRoot syntax, string schema, string table)
        {
            return syntax.Index().OnTable(table).InSchema(schema);
        }

        public static ICreateIndexOptionsSyntax IncludeColumns(this ICreateIndexOptionsSyntax syntax, params string[] columns)
        {
            return columns
                .Select(syntax.Include)
                .Last();
        }

        public static ICreateIndexOnColumnSyntax OnColumns(this ICreateIndexOnColumnSyntax syntax, params string[] columns)
        {
            return columns
                .Select(x => syntax.OnColumn(x).Ascending())
                .Last();
        }

        public static ICreateIndexOptionsSyntax Include(this ICreateIndexOnColumnSyntax syntax, params string[] columns)
        {
            return syntax.WithOptions().IncludeColumns(columns);
        }

        public static IDataTableSyntax Table(this IDataSyntax syntax, string schema, string table)
        {
            return syntax.InSchema(schema).InTable(table);
        }

        public static bool ForeignKeyExists(this IDataSyntax syntax, string name)
        {
            return syntax.Table("INFORMATION_SCHEMA", "REFERENTIAL_CONSTRAINTS").Exists(new { CONSTRAINT_NAME = name });
        }

        public static bool TableConstraintExists(this IDataSyntax syntax, string name)
        {
            return syntax.Table("INFORMATION_SCHEMA", "TABLE_CONSTRAINTS").Exists(new { CONSTRAINT_NAME = name });
        }

        public static bool UniqueConstraintExists(this IDataSyntax syntax, string name)
        {
            return syntax.Table("INFORMATION_SCHEMA", "TABLE_CONSTRAINTS").Exists(new { CONSTRAINT_NAME = name, CONSTRAINT_TYPE = "UNIQUE" });
        }

        public static bool IndexExists(this IDataSyntax syntax, string name)
        {
            return syntax.Table("sys", "indexes").Exists(new {name });
        }
    }
}