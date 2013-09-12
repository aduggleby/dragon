using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.SQL
{
    public class SqlBuilderHelper
    {

        public static string BuildExistsTable(TableMetadata metadata)
        {
            var name = metadata.TableName;
            var schema = metadata.Schema;
            
            return string.Format("SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[{1}].[{0}]') AND type in (N'U')",name, schema);
        }

        public static string BuildCreate(
            TableMetadata metadata,
            bool onlyIfNotExists = false)
        {
            StringBuilder sqlCreate = new StringBuilder();
            StringBuilder sqlIndexes = new StringBuilder();

            var name = metadata.TableName;
            var schema = metadata.Schema;

            var keys = metadata.Properties.Where(x => x.IsPK).ToList();
            var nonkeys = metadata.Properties.Where(x => !x.IsPK).ToList();

            if (onlyIfNotExists)
            {
                sqlCreate.AppendFormat("IF NOT EXISTS (" +
                    "SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{1}].[{0}]') AND type in (N'U'))\r\nBEGIN\r\n", name, schema);
            }

            sqlCreate.AppendFormat("CREATE TABLE [{1}].[{0}](\r\n", name, schema??"dbo");

            if (!keys.Any() || keys.Count() > 1)
            {
                throw new Exception("This only support entites with exactly one key property at the moment.");
            }

            var pk = keys.First();
            sqlCreate.AppendFormat("   [{0}] {1} NOT NULL,\r\n", pk.ColumnName, pk.SqlKeyTypeString);

            var keyColumns = string.Join(",", keys.Select(x => string.Format("[{0}] ASC", x.ColumnName)).ToArray());
            var constraint = string.Format("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ( {1} )", name, keyColumns);

            foreach (var propertyMetadata in nonkeys)
            {
                sqlCreate.AppendFormat("   [{0}] {1} {2},\r\n", 
                    propertyMetadata.ColumnName, 
                    propertyMetadata.SqlTypeString, 
                    propertyMetadata.Nullable ? "NULL" : "NOT NULL");

                // INDEX
                if (propertyMetadata.Indexed)
                {
                    sqlIndexes.AppendFormat("CREATE NONCLUSTERED INDEX IX_{0}_{1} ON {0}({1})\r\n", name, propertyMetadata.ColumnName);
                }
            }

            sqlCreate.AppendLine(constraint);

            sqlCreate.AppendFormat(");\r\n");
            sqlCreate.Append(sqlIndexes);

            if (onlyIfNotExists)
            {
                sqlCreate.AppendFormat("END\r\n");
            }

            return sqlCreate.ToString();
        }

        public static string BuildExists(
            TableMetadata metadata)
        {
            var schema = GetSchema(metadata);

            return string.Format("SELECT (1) FROM {0}[{1}] WHERE {2}",
                schema,
                metadata.TableName,
                BuildWhereClause(metadata));
        }

        public static string BuildSelectStar(
         TableMetadata metadata)
        {
            return BuildSelectWithCustomColumns(metadata, "*");
        }

        public static string BuildSelectCount(
         TableMetadata metadata)
        {
            return BuildSelectWithCustomColumns(metadata, "COUNT(0)");
        }
        

        public static string BuildSelectWithCustomColumns(
        TableMetadata metadata,
        string customColumns)
        {
            var schema = GetSchema(metadata);

            return string.Format("SELECT {0} FROM {1}[{2}]",
                customColumns,
                schema,
                metadata.TableName);
        }

        public static string BuildSelect(
            TableMetadata metadata)
        {
            var schema = GetSchema(metadata);

            return string.Format("SELECT {0} FROM {1}[{2}]",
                BuildMappedColumnList(metadata),
                schema,
                metadata.TableName);
        }

        public static string BuildSelect(
            TableMetadata metadata,
            Dictionary<string, object> values,
            ref Dictionary<string, object> parameters)
        {
            var schema = String.Empty;
            if (!String.IsNullOrWhiteSpace(metadata.Schema))
            {
                schema = string.Format("[{0}].", metadata.Schema);
            }

            return string.Format("SELECT {0} FROM {1}[{2}] WHERE {3}",
                BuildMappedColumnList(metadata),
                schema,
                metadata.TableName,
                BuildWhereClause(metadata, values, ref parameters));
        }

        public static string BuildInsert(
            TableMetadata metadata,
            bool withoutKeys = true)
        {
            var schema = GetSchema(metadata);

            return string.Format("INSERT INTO {0}[{1}] ({2}) VALUES ({3})",
                schema,
                metadata.TableName,
                BuildColumnList(metadata, withoutKeys: withoutKeys),
                BuildParameterList(metadata, withoutKeys: withoutKeys)
                );
        }

        public static string BuildUpdate(
            TableMetadata metadata,
            bool withoutKeys = true)
        {
            var schema = GetSchema(metadata);

            return string.Format("UPDATE {0}[{1}] SET {2} WHERE {3}",
                schema,
                metadata.TableName,
                BuildParameterSettingList(metadata, withoutKeys: withoutKeys),
                BuildWhereClause(metadata)
                );
        }

        public static string BuildDelete(
            TableMetadata metadata)
        {
            var schema = GetSchema(metadata);

            return string.Format("DELETE FROM {0}[{1}] WHERE {2}",
                schema,
                metadata.TableName,
                BuildWhereClause(metadata)
                );
        }

        public static string BuildColumnList(
            TableMetadata metadata,
            bool withoutKeys = false)
        {
            return string.Join(",", metadata
                .Properties
                .Where(x => !withoutKeys || !x.IsPK)
                .OrderBy(x => x.ColumnName)
                .Select(x => string.Format("[{0}]", x.ColumnName)).ToArray());
        }

        public static string BuildMappedColumnList(
          TableMetadata metadata,
          bool withoutKeys = false)
        {
            return string.Join(",", metadata
                .Properties
                .Where(x => !withoutKeys || !x.IsPK)
                .OrderBy(x => x.ColumnName)
                .Select(x => string.Format("[{0}] AS '{1}'", x.ColumnName, x.PropertyName)).ToArray());
        }


        public static string BuildParameterList(
            TableMetadata metadata,
            bool withoutKeys = false)
        {
            if (metadata == null)
                throw new ArgumentException("No metadata provided (null).", "metadata");

            var insertProperties = metadata
                .Properties
                .Where(x => !withoutKeys || !x.IsPK);

            return string.Join(",", 
                insertProperties
                .OrderBy(x => x.ColumnName).Select(GetVariableName).ToArray());
        }

        public static string BuildParameterSettingList(
            TableMetadata metadata,
            bool withoutKeys = true)
        {
            if (metadata == null)
                throw new ArgumentException("No metadata provided (null).", "metadata");

            var insertProperties = metadata
                .Properties
                .Where(x => !withoutKeys || !x.IsPK);

            return string.Join(",",
                insertProperties
                .OrderBy(x => x.ColumnName)
                .Select(x=>string.Format("[{0}]={1}", x.ColumnName, GetVariableName(x)))
                .ToArray());
        }

        public static string BuildWhereClause(
            TableMetadata metadata,
            Dictionary<string, object> values,
            ref Dictionary<string, object> parameters)
        {
            if (metadata == null)
                throw new ArgumentException("No metadata provided (null).", "metadata");

            if (!values.Any())
                throw new ArgumentException("At least one key value is required.", "values");

            var sb = new StringBuilder();
            var first = true;
            foreach (var where in values)
            {
                var propMetadata = metadata.Properties.FirstOrDefault(x => x.PropertyName.Equals(where.Key));

                if (propMetadata == null)
                    throw new ArgumentException(string.Format("Could not find property {0} in metadata.", where.Key), "values");

                if (!first) sb.Append(" AND ");

                var propertyName = FindUniqueNameInDictionary(GetVariableName(propMetadata), parameters);

                if (!(where.Value is string) && where.Value is IEnumerable)
                {
                    sb.AppendFormat("[{0}] IN {1}", propMetadata.ColumnName, propertyName);
                    parameters.Add(propertyName, where.Value);
                }
                else
                {
                    sb.AppendFormat("[{0}]={1}", propMetadata.ColumnName, propertyName);
                    parameters.Add(propertyName, where.Value);
                }

                first = false;
            }

            return sb.ToString();
        }

        public static string BuildWhereClause(
            TableMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentException("No metadata provided (null).", "metadata");

            var sb = new StringBuilder();
            var first = true;
            foreach (var propMetadata in metadata.Properties.Where(x=>x.IsPK))
            {
                if (!first) sb.Append(" AND ");

                var propertyName = GetVariableName(propMetadata);

                sb.AppendFormat("[{0}]={1}", propMetadata.ColumnName, propertyName);

                first = false;
            }

            return sb.ToString();
        }


        public static void BuildParameters(
           TableMetadata metadata,
           Dictionary<string, object> values,
           ref Dictionary<string, object> parameters)
        {
            if (metadata == null)
                throw new ArgumentException("No metadata provided (null).", "metadata");

            if (!values.Any())
                throw new ArgumentException("At least one key value is required.", "values");

            foreach (var where in values)
            {
                var propMetadata = metadata.Properties.FirstOrDefault(x => x.PropertyName.Equals(where.Key));

                if (propMetadata != null)
                {
                    var propertyName = GetVariableName(propMetadata);
                    parameters.Add(propertyName, where.Value);
                }
            }
        }

        public static string FindUniqueNameInDictionary(string baseName, Dictionary<string, object> dict)
        {
            var name = baseName;
            var counter = 1;
            while (dict.ContainsKey(name))
            {
                name = baseName + (++counter);
            }
            return name;
        }

        public static string GetVariableName(PropertyMetadata propMetadata)
        {
            return string.Format("@{0}", propMetadata.PropertyName);
        }

        public static string GetSchema(TableMetadata metadata)
        {
            var schema = String.Empty;
            if (!String.IsNullOrWhiteSpace(metadata.Schema))
            {
                schema = string.Format("[{0}].", metadata.Schema);
            }
            return schema;
        }
    }
}
