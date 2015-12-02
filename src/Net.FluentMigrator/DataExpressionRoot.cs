using System;
using System.Reflection;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner.Processors;
using Net.Collections;
using Net.Text;

namespace Net.FluentMigrator
{
    public class DataExpressionRoot :
        IDataSyntax,
        IDataTableSyntax,
        IDataSchemaSyntax
    {
        private readonly IMigrationContext _context;
        private readonly TableDefinition _table;

        public DataExpressionRoot(IMigrationContext migrationContext)
        {
            _context = migrationContext;
            _table = new TableDefinition { SchemaName = "dbo" };
        }

        public IDataSchemaSyntax InSchema(string schemaName)
        {
            _table.SchemaName = schemaName;
            return this;
        }

        public IDataTableSyntax InTable(string tableName)
        {
            _table.Name = tableName;
            return this;
        }

        public bool Exists(object row)
        {
            string objectToWhereClause = ObjectToWhereClause(row);
            //Console.WriteLine("Checking existing data for: SELECT * FROM [{0}].[{1}] WHERE {2}".FormatWith(table.SchemaName, table.Name, objectToWhereClause));
            return ((ProcessorBase)_context.QuerySchema).Exists("SELECT 1 FROM [{0}].[{1}] WHERE {2}", _table.SchemaName, _table.Name, objectToWhereClause);
        }

        private static string ObjectToWhereClause(object row)
        {
            return row.GetType().GetProperties().ConcatToString(p => ToEqualsClause(row, p), " AND ");
        }

        private static string ToEqualsClause(object row, PropertyInfo p)
        {
            return "[{0}]={1}".FormatWith(p.Name, EscapeValue(p.PropertyType, p.GetValue(row, null)));
        }

        private static string EscapeValue(Type propertyType, object value)
        {
            return new[] { typeof(int), typeof(short), typeof(long), typeof(decimal) }.IndexOf(propertyType) > -1 ? value.ToString() : string.Format("'{0}'", value.ToString().Replace("'", "''"));
        }
    }
}