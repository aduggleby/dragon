using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dragon.Data.Attributes;

namespace Dragon.Data
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

            foreach (var p in metadata.Properties)
            {
                p.AfterPropertyMetadataSet(p, metadata);
            }

            foreach (var p in metadata.Properties)
            {
                SqlMetadataForProperty(p);
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

            metadata.AfterPropertyMetadataSet = (propmd, tablemd) =>
            {
                propmd.IsOnlyPK = propmd.IsPK &&
                                  tablemd.Properties.Count(x => x.IsPK) == 1;

            };

        }

        public static void SqlMetadataForProperty(PropertyMetadata metadata)
        {
            var actualType = metadata.PropertyInfo.PropertyType;
            var isNullableType = false;

            if (actualType.GetTypeInfo().IsGenericType &&
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

            if (metadata.IsTimestamp && actualType == typeof(byte[]))
            {
                sqlType = "[TIMESTAMP]";
            } 
            else if (actualType == typeof(int))
            {
                sqlType = "[INT]";
            }
            else if (actualType == typeof(byte))
            {
                sqlType = "[TINYINT]";
            }
            else if (actualType == typeof(Int16))
            {
                sqlType = "[SMALLINT]";
            }
            else if (actualType == typeof(Int64))
            {
                sqlType = "[BIGINT]";
            }
            else if (actualType == typeof(string))
            {
                sqlType = string.Format("[NVARCHAR]({0})", metadata.Length);
            }
            else if (actualType == typeof(Guid))
            {
                sqlType = "[UNIQUEIDENTIFIER]";
            }
            else if (actualType == typeof(DateTime))
            {
                sqlType = "[DATETIME]";
            }
            else if (actualType == typeof(decimal))
            {
                sqlType = "[DECIMAL](18,4)";
            }
            else if (actualType == typeof(Single))
            {
                sqlType = "[REAL]";
            }
            else if (actualType == typeof(Boolean))
            {
                sqlType = "[BIT]";
            }
            else if (actualType == typeof(byte[]))
            {
                sqlType = "[VARBINARY](MAX)";
            }
            else if (actualType == typeof(double))
            {
                sqlType = "[FLOAT]";
            }
            else if (actualType.GetTypeInfo().IsEnum)
            {
                sqlType = "[INT]";
            }
            else
            {
                throw new Exception(string.Format("Unknown data type '{0}'~'{1}'", metadata.PropertyInfo.PropertyType.Name, actualType));
            }

            metadata.SqlTypeString = sqlType;
            metadata.Nullable = isNullableType;
            metadata.IsAutoIncrementingPK = false;

            if (metadata.IsPK)
            {
                if (actualType == typeof(Guid))
                {
                  metadata.SqlKeyTypeString = "[UNIQUEIDENTIFIER]";
                }
                else if (actualType== typeof (int))
                {
                    if (metadata.IsOnlyPK) // if multiple ints as keys don't autoincrement them
                    {
                        metadata.SqlKeyTypeString = "[int] IDENTITY(1,1)";
                        metadata.IsAutoIncrementingPK = metadata.IsPK; // ints autoincrement
                    }
                    else
                    {
                        metadata.SqlKeyTypeString = sqlType;
                    }
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
            Func<TRet> ifNull) where TAtt : Attribute
        {
            TAtt att = (TAtt)tSubject.GetTypeInfo().GetCustomAttributes(true).SingleOrDefault(x => x.GetType() == typeof(TAtt));
            return IfNotNullElse<TAtt, TRet>(att, ifNotNull, ifNull);
        }

        public static TRet IfAttributeElse<TAtt, TRet>(
           PropertyInfo tSubject,
           Type tAttribute,
           Func<TAtt, TRet> ifNotNull,
           Func<TRet> ifNull) where TAtt : Attribute
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
