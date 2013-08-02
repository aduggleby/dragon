using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dragon.Common.Attributes.Data;

namespace Dragon.SQL
{
    public static class MetadataHelper
    {

        public static void MetadataForClass(Type t, ref TableMetadata metadata)
        {
            metadata.ClassName = t.Name;

            metadata.TableName = IfAttributeElse<TableAttribute, string>(
                t, typeof(TableAttribute), a => a.Name, () => t.Name);

            metadata.Schema = IfAttributeElse<SchemaAttribute, string>(
                t, typeof(SchemaAttribute), a => a.Name, () => null);

            MetadataForProperties(t, ref metadata);
        }

        public static void MetadataForProperties(Type t, ref TableMetadata metadata)
        {
            foreach (var property in t.GetProperties().Where(x => x.CanWrite && x.CanRead))
            {
                var propmetadata = new PropertyMetadata();
                var hasNoColumnAtt = (null != property.GetCustomAttributes(true).SingleOrDefault(x => x.GetType() == typeof(NoColumnAttribute)));

                if (!hasNoColumnAtt)
                {
                    metadata.Properties.Add(propmetadata);
                    MetadataForProperty(property, ref propmetadata);
                }
            }
        }

        public static void MetadataForProperty(PropertyInfo pi, ref PropertyMetadata metadata)
        {
            metadata.PropertyInfo = pi;
            metadata.IsPK = pi.GetCustomAttributes(true).Any(a => a is KeyAttribute);
            metadata.PropertyName = pi.Name;
            metadata.ColumnName = IfAttributeElse<ColumnAttribute, string>(
                pi, typeof(ColumnAttribute), a => a.Name, () => pi.Name);
            metadata.Length = IfAttributeElse<LengthAttribute, string>(
                pi, typeof(LengthAttribute), a => a.Length, () => "50");
            metadata.Indexed = pi.GetCustomAttributes(true).Any(a => a is IndexAttribute);

            SqlMetadataForProperty(ref metadata);
        }

        public static void SqlMetadataForProperty(ref PropertyMetadata metadata)
        {
            var actualType = metadata.PropertyInfo.PropertyType;
            var isNullableType = false;

            if (actualType.IsGenericType &&
                actualType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var genericParams = actualType.GetGenericArguments(); // can only be one for Nullable
                actualType = genericParams[0];
                isNullableType = true;
            }
            else if (actualType == typeof(string) ||
                actualType == typeof(byte[]))
            {
                isNullableType = true;
            }

            var sqlType = string.Empty;

            if (actualType == typeof(int))
            {
                sqlType = "[INT]";
                isNullableType = true;
            }
            else if (actualType == typeof(string))
            {
                sqlType = string.Format("[NVARCHAR]({0})", metadata.Length);
            }
            else if (actualType == typeof(Guid))
            {
                sqlType = "[UNIQUEIDENTIFIER]";
                isNullableType = true;
            }
            else if (actualType == typeof(DateTime))
            {
                sqlType = "[DATETIME]";
                isNullableType = true;
            }
            else if (actualType == typeof(decimal))
            {
                sqlType = "[DECIMAL](18,4)";
                isNullableType = true;

            }
            else if (actualType == typeof(Boolean))
            {
                sqlType = "[BIT]";
                isNullableType = true;
            }
            else if (actualType == typeof(byte[]))
            {
                sqlType = "[VARBINARY](MAX)";
                isNullableType = true;

            }
            else if (actualType.IsEnum)
            {
                sqlType = "[INT]";
                isNullableType = true;
            }
            else
            {
                throw new Exception(string.Format("Unknown data type '{0}'", metadata.PropertyInfo.PropertyType.Name));
            }

            metadata.SqlTypeString = sqlType;
            metadata.Nullable = isNullableType;

            if (metadata.IsPK)
            {
                if (actualType == typeof(Guid))
                {
                  metadata.SqlKeyTypeString = "[UNIQUEIDENTIFIER]";
                }
                else if (actualType== typeof (int))
                {
                    metadata.SqlKeyTypeString = "[int] IDENTITY(1,1)";
                }
                else
                {
                    metadata.SqlKeyTypeString = sqlType;
                }
            }
        }

        public static TRet IfAttributeElse<TAtt, TRet>(
            Type tSubject,
            Type tAttribute,
            Func<TAtt, TRet> ifNotNull,
            Func<TRet> ifNull)
        {
            TAtt att = (TAtt)tSubject.GetCustomAttributes(true).SingleOrDefault(x => x.GetType() == typeof(TAtt));
            return IfNotNullElse<TAtt, TRet>(att, ifNotNull, ifNull);
        }

        public static TRet IfAttributeElse<TAtt, TRet>(
           PropertyInfo tSubject,
           Type tAttribute,
           Func<TAtt, TRet> ifNotNull,
           Func<TRet> ifNull)
        {
            TAtt att = (TAtt)tSubject.GetCustomAttributes(true).SingleOrDefault(x => x.GetType() == typeof(TAtt));
            return IfNotNullElse<TAtt, TRet>(att, ifNotNull, ifNull);
        }

        public static TRet IfNotNullElse<TTest, TRet>(TTest test, Func<TTest, TRet> ifNotNull, Func<TRet> ifNull)
        {
            return test == null ? ifNull() : ifNotNull(test);
        }

    }
}
