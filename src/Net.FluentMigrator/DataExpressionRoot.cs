using System;
using System.Reflection;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.Processors;
using Net.Collections;

namespace Net.FluentMigrator
{
    public class DataExpressionRoot :
        IDataSyntax,
        IDataTableSyntax,
        IDataSchemaSyntax
    {
        readonly IMigrationContext _context;
        readonly TableDefinition _table;

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
            return ((ProcessorBase)_context.QuerySchema).Exists("SELECT * FROM [{0}].[{1}] WHERE {2}", _table.SchemaName, _table.Name, objectToWhereClause);
        }

        static string ObjectToWhereClause(object row)
        {
            return row.GetType().GetProperties().ConcatToString(p => ToEqualsClause(row, p), " AND ");
        }

        static string ToEqualsClause(object row, PropertyInfo p)
        {
            return $"[{p.Name}]={EscapeValue(p.PropertyType, p.GetValue(row, null))}";
        }

        static string EscapeValue(Type propertyType, object value)
        {
            return new[] { typeof(int), typeof(short), typeof(long), typeof(decimal) }.IndexOf(propertyType) > -1 ? value.ToString() : $"'{value.ToString().Replace("'", "''")}'";
        }
    }

    public class TableDefinition
    {
        public string SchemaName { get; set; }
        public string Name { get; set; }
    }
}