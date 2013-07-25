using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Dapper;
using Dragon.Common.Attributes.Data;

namespace Dapper
{
    // https://raw.github.com/SamSaffron/dapper-dot-net/master/Dapper.Contrib/SqlMapperExtensions.cs
    public static class SqlMapperExtensions
    {
        public interface IProxy
        {
            bool IsDirty { get; set; }
        }

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static readonly ConcurrentDictionary<PropertyInfo, string> PropertyLengths = new ConcurrentDictionary<PropertyInfo, string>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetQueries = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetAllQueries = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static IEnumerable<PropertyInfo> KeyPropertiesCache(Type type)
        {

            IEnumerable<PropertyInfo> pi;
            if (KeyProperties.TryGetValue(type.TypeHandle, out pi))
            {
                return pi;
            }

            var allProperties = TypePropertiesCache(type);
            var keyProperties = allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

            if (keyProperties.Count == 0)
            {
                var idProp = allProperties.Where(p => p.Name.ToLower() == "id").FirstOrDefault();
                if (idProp != null)
                {
                    keyProperties.Add(idProp);
                }
            }

            KeyProperties[type.TypeHandle] = keyProperties;
            return keyProperties;
        }

        private static IEnumerable<PropertyInfo> TypePropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pis;
            if (TypeProperties.TryGetValue(type.TypeHandle, out pis))
            {
                return pis;
            }

            var properties = type.GetProperties().Where(x => x.CanWrite && x.CanRead);
            TypeProperties[type.TypeHandle] = properties;
            return properties;
        }

        private static string LengthPropertiesCache(PropertyInfo pi)
        {
            string length = "0";
            if (PropertyLengths.TryGetValue(pi, out length))
            {
                return length;
            }

            var lengthAttributes = pi.GetCustomAttributes(true)
                .Where(a => a is LengthAttribute)
                .OfType<LengthAttribute>();

            if (lengthAttributes.Count() == 0)
            {
                length = "50" /* default */;
            }
            else if (lengthAttributes.Count() == 1)
            {
                length = lengthAttributes.First().Length;
                PropertyLengths[pi] = length;
            }
            else
            {
                throw new Exception("Only one LengthAttribute allowed per property.");
            }

            return length;
        }

        /// <summary>
        /// Returns a single entity by a single id from table "Ts". T must be of interface type. 
        /// Id must be marked with [Key] attribute.
        /// Created entity is tracked/intercepted for changes and used by the Update() extension. 
        /// </summary>
        /// <typeparam name="T">Interface type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <returns>Entity of T</returns>
        public static T Get<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            string sql;
            if (!GetQueries.TryGetValue(type.TypeHandle, out sql))
            {
                var keys = KeyPropertiesCache(type);
                if (keys.Count() > 1)
                    throw new DataException("Get<T> only supports an entity with a single [Key] property");
                if (keys.Count() == 0)
                    throw new DataException("Get<T> only supports en entity with a [Key] property");

                var onlyKey = keys.First();

                var name = GetTableName(type);

                // TODO: pluralizer 
                // TODO: query information schema and only select fields that are both in information schema and underlying class / interface 
                sql = "select * from [" + name + "] where " + onlyKey.Name + " = @id";
                GetQueries[type.TypeHandle] = sql;
            }

            var dynParms = new DynamicParameters();
            dynParms.Add("@id", id);

            T obj = null;

            if (type.IsInterface)
            {
                var res = connection.Query(sql, dynParms).FirstOrDefault() as IDictionary<string, object>;

                if (res == null)
                    return (T)((object)null);

                obj = ProxyGenerator.GetInterfaceProxy<T>();

                foreach (var property in TypePropertiesCache(type))
                {
                    var val = res[property.Name];
                    property.SetValue(obj, val, null);
                }

                ((IProxy)obj).IsDirty = false;   //reset change tracking and return
            }
            else
            {
                obj = connection.Query<T>(sql, dynParms, transaction: transaction, commandTimeout: commandTimeout).FirstOrDefault();
            }
            return obj;
        }

        /// <summary>
        /// Returns all entitities from table "Ts". 
        /// Id must be marked with [Key] attribute.
        /// </summary>
        /// <typeparam name="T">Interface type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <returns>Entity of T</returns>
        public static IEnumerable<T> GetAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            string sql;
            if (!GetAllQueries.TryGetValue(type.TypeHandle, out sql))
            {
                var name = GetTableName(type);
                sql = "SELECT * FROM [" + name + "]";
                GetAllQueries[type.TypeHandle] = sql;
            }

            return connection.Query<T>(sql, transaction: transaction, commandTimeout: commandTimeout);
        }

        public static string GetTableName(Type type)
        {
            string name;
            if (!TypeTableName.TryGetValue(type.TypeHandle, out name))
            {
                name = type.Name; // +"s";
                //if (type.IsInterface && name.StartsWith("I"))
                //name = name.Substring(1);

                //NOTE: This as dynamic trick should be able to handle both our own Table-attribute as well as the one in EntityFramework 
                var tableattr = type.GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as
                    dynamic;
                if (tableattr != null)
                    name = tableattr.Name;
                TypeTableName[type.TypeHandle] = name;
            }
            return name;
        }

        /// <summary>
        /// Inserts an entity into table "Ts" and returns identity id.
        /// </summary>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToInsert">Entity to insert</param>
        /// <returns>Identity of inserted entity</returns>
        public static TKey Insert<T, TKey>(this IDbConnection connection,
            T entityToInsert,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            bool letDbGenerateKey = true) where T : class
        {

            var type = typeof(T);

            var name = GetTableName(type);

            var sb = new StringBuilder(null);
            sb.AppendFormat("insert into [{0}] (", name);

            var allProperties = TypePropertiesCache(type).ToList();
            var keyProperties = KeyPropertiesCache(type).ToList();
            var allPropertiesExceptKey = allProperties.Except(keyProperties).ToList();
            var columnSet = letDbGenerateKey ? allPropertiesExceptKey : allProperties;

            for (var i = 0; i < columnSet.Count(); i++)
            {
                var property = columnSet.ElementAt(i);
                sb.Append(string.Format("[{0}]", property.Name));
                if (i < columnSet.Count() - 1)
                    sb.Append(", ");
            }
            sb.Append(") values (");

            for (var i = 0; i < columnSet.Count(); i++)
            {
                var property = columnSet.ElementAt(i);
                sb.AppendFormat("@{0}", property.Name);
                if (i < columnSet.Count() - 1)
                    sb.Append(", ");
            }
            sb.Append(") ");
            
            connection.Execute(sb.ToString(), entityToInsert, transaction: transaction, commandTimeout: commandTimeout);

            if (letDbGenerateKey)
            {
                //NOTE: would prefer to use IDENT_CURRENT('tablename') or IDENT_SCOPE but these are not available on SQLCE
                var r = connection.Query("select @@IDENTITY id", transaction: transaction,
                                         commandTimeout: commandTimeout);
                return (TKey)r.First().id;
            }
            else
            {
                return (TKey)keyProperties.First().GetValue(entityToInsert, null);
            }

        }

        /// <summary>
        /// Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToUpdate">Entity to be updated</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public static bool Update<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var proxy = entityToUpdate as IProxy;
            if (proxy != null)
            {
                if (!proxy.IsDirty) return false;
            }

            var type = typeof(T);

            var keyProperties = KeyPropertiesCache(type);
            if (keyProperties.Count() == 0)
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(type);

            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE [{0}] SET ", name);

            var allProperties = TypePropertiesCache(type);
            var nonIdProps = allProperties.Where(a => !keyProperties.Contains(a));

            for (var i = 0; i < nonIdProps.Count(); i++)
            {
                var property = nonIdProps.ElementAt(i);
                sb.AppendFormat("[{0}] = @{1}", property.Name, property.Name);
                if (i < nonIdProps.Count() - 1)
                    sb.AppendFormat(", ");
            }
            sb.Append(" WHERE ");
            for (var i = 0; i < keyProperties.Count(); i++)
            {
                var property = keyProperties.ElementAt(i);
                sb.AppendFormat("[{0}] = @{1}", property.Name, property.Name);
                if (i < keyProperties.Count() - 1)
                    sb.AppendFormat(" AND ");
            }
            var updated = connection.Execute(sb.ToString(), entityToUpdate, commandTimeout: commandTimeout, transaction: transaction);
            return updated > 0;
        }

        public static TKey UpdateOrInsert<T, TKey>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null,
                                     int? commandTimeout = null,
            bool letDbGenerateKey = true) where T : class
        {
            var updated = connection.Update(entityToUpdate, transaction, commandTimeout);
            if (updated)
            {
                var type = typeof(T);
                var keyProperties = KeyPropertiesCache(type);

                return (TKey)keyProperties.First().GetValue(entityToUpdate, new object[0]);
            }
            else
            {
                return connection.Insert<T, TKey>(entityToUpdate, transaction, commandTimeout, letDbGenerateKey: letDbGenerateKey);
            }
        }

        /// <summary>
        /// Delete entity in table "Ts".
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToDelete">Entity to delete</param>
        /// <returns>true if deleted, false if not found</returns>
        public static bool Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);

            var keyProperties = KeyPropertiesCache(type);
            if (keyProperties.Count() == 0)
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(type);

            var sb = new StringBuilder();
            sb.AppendFormat("delete from {0} where ", name);

            for (var i = 0; i < keyProperties.Count(); i++)
            {
                var property = keyProperties.ElementAt(i);
                sb.AppendFormat("{0} = @{1}", property.Name, property.Name);
                if (i < keyProperties.Count() - 1)
                    sb.AppendFormat(" and ");
            }
            var deleted = connection.Execute(sb.ToString(), entityToDelete, transaction: transaction, commandTimeout: commandTimeout);
            return deleted > 0;
        }

        public static bool Exists<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);

            var keyProperties = KeyPropertiesCache(type);
            if (keyProperties.Count() == 0)
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(type);

            var sb = new StringBuilder();
            sb.AppendFormat("SELECT (1) FROM {0} WHERE", name);

            for (var i = 0; i < keyProperties.Count(); i++)
            {
                var property = keyProperties.ElementAt(i);
                sb.AppendFormat("{0} = @{1}", property.Name, property.Name);
                if (i < keyProperties.Count() - 1)
                    sb.AppendFormat(" AND ");
            }

            var noOfRowsMatching = connection.Execute(sb.ToString(), entityToDelete, transaction: transaction, commandTimeout: commandTimeout);
            return noOfRowsMatching > 0;
        }

        #region TableDropper

        public static void DropTableIfExists<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            DropTable<T>(connection, true, transaction, commandTimeout);
        }

        public static void DropTableIfExists(this IDbConnection connection, string name, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DropTable(connection, name, true, transaction, commandTimeout);
        }

        public static void DropTable<T>(this IDbConnection connection, bool onlyIfExists, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var sql = DropTableSQL<T>(onlyIfExists);

            connection.Execute(sql, transaction: transaction, commandTimeout: commandTimeout);
        }

        public static void DropTable(this IDbConnection connection, string name, bool onlyIfExists, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var sql = DropTableSQL(name, onlyIfExists);

            connection.Execute(sql, transaction: transaction, commandTimeout: commandTimeout);
        }

        private static string DropTableSQL<T>(bool onlyIfExists) where T : class
        {
            var type = typeof(T);
            var name = SqlMapperExtensions.GetTableName(type);

            return DropTableSQL(name, onlyIfExists);
        }

        private static string DropTableSQL(string name, bool onlyIfExists)
        {
            StringBuilder sqlDrop = new StringBuilder();

            if (onlyIfExists)
            {
                sqlDrop.AppendFormat("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}')", name);
            }

            sqlDrop.AppendFormat("DROP TABLE [dbo].[{0}]", name);

            return sqlDrop.ToString();
        }
        #endregion

        #region TableCreator

        public static void CreateTableIfNotExists<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            CreateTable<T>(connection, true, transaction, commandTimeout);
        }

        public static void CreateTable<T>(this IDbConnection connection, bool onlyIfExists, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var sql = CreateTableSQL<T>(onlyIfExists);

            connection.Execute(sql, transaction: transaction, commandTimeout: commandTimeout);
        }

        private static string CreateTableSQL<T>(bool onlyIfExists) where T : class
        {
            var type = typeof(T);

            var keys = KeyPropertiesCache(type);
            if (keys.Count() == 0)
                throw new DataException("CreateTableSQL<T> only supports an entity with a [Key] property");

            var name = SqlMapperExtensions.GetTableName(type);

            StringBuilder sqlCreate = new StringBuilder();
            StringBuilder sqlIndexes = new StringBuilder();

            if (onlyIfExists)
            {
                sqlCreate.AppendFormat("IF NOT EXISTS (" +
                    "SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))\r\nBEGIN\r\n", name);
            }

            sqlCreate.AppendFormat("CREATE TABLE [dbo].[{0}](\r\n", name);

            foreach (var key in keys)
            {
                var keySqlType = string.Empty;

                var keyType = key.PropertyType;

                if (keyType.IsGenericType && keyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var _genericParams = keyType.GetGenericArguments(); // can only be one for Nullable
                    keyType = _genericParams[0];
                }

                if (keyType == typeof(Guid))
                {
                    keySqlType = "[UNIQUEIDENTIFIER]";
                }
                else if (keyType == typeof(int))
                {
                    keySqlType = "[int]";

                    if (keys.Count() == 1)
                    {
                        keySqlType += " IDENTITY(1,1)";
                    }
                }
                else
                {
                    throw new Exception("Only Guid or Int accepted as key.");
                }

                sqlCreate.AppendFormat("   [{0}] {1} NOT NULL,\r\n", key.Name, keySqlType);
            }

            var keyColumns = string.Join(",", keys.Select(x => string.Format("[{0}] ASC", x.Name)).ToArray());
            var constraint = string.Format("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ( {1} )", name, keyColumns);

            foreach (var property in TypePropertiesCache(type).Except(keys))
            {
                var actualType = property.PropertyType;
                var isNullableType = false;

                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var _genericParams = property.PropertyType.GetGenericArguments(); // can only be one for Nullable
                    actualType = _genericParams[0];
                    isNullableType = true;
                }
                else if (property.PropertyType == typeof(string) ||
                    property.PropertyType == typeof(byte[]))
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
                    sqlType = string.Format("[NVARCHAR]({0})", LengthPropertiesCache(property));
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
                    throw new Exception(string.Format("Unknown data type '{0}'", property.PropertyType.Name));
                }

                var nullOrNot = isNullableType ? "NULL" : "NOT NULL";

                sqlCreate.AppendFormat("   [{0}] {1} {2},\r\n", property.Name, sqlType, nullOrNot);

                // INDEX
                if (property.GetCustomAttributes(true).OfType<IndexAttribute>().Count() > 0)
                {
                    sqlIndexes.AppendFormat("CREATE NONCLUSTERED INDEX IX_{0}_{1} ON {0}({1})\r\n", name, property.Name);
                }
            }

            sqlCreate.AppendLine(constraint);

            sqlCreate.AppendFormat(");\r\n");
            sqlCreate.Append(sqlIndexes);

            if (onlyIfExists)
            {
                sqlCreate.AppendFormat("END\r\n");
            }

            return sqlCreate.ToString();
        }


        #endregion

        class ProxyGenerator
        {
            private static readonly Dictionary<Type, object> TypeCache = new Dictionary<Type, object>();

            private static AssemblyBuilder GetAsmBuilder(string name)
            {
                var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName { Name = name },
                    AssemblyBuilderAccess.Run);       //NOTE: to save, use RunAndSave

                return assemblyBuilder;
            }

            public static T GetClassProxy<T>()
            {
                // A class proxy could be implemented if all properties are virtual
                //  otherwise there is a pretty dangerous case where internal actions will not update dirty tracking
                throw new NotImplementedException();
            }
            
            public static T GetInterfaceProxy<T>()
            {
                Type typeOfT = typeof(T);

                object k;
                if (TypeCache.TryGetValue(typeOfT, out k))
                {
                    return (T)k;
                }
                var assemblyBuilder = GetAsmBuilder(typeOfT.Name);

                var moduleBuilder = assemblyBuilder.DefineDynamicModule("SqlMapperExtensions." + typeOfT.Name); //NOTE: to save, add "asdasd.dll" parameter

                var interfaceType = typeof(Dapper.SqlMapperExtensions.IProxy);
                var typeBuilder = moduleBuilder.DefineType(typeOfT.Name + "_" + Guid.NewGuid(),
                    TypeAttributes.Public | TypeAttributes.Class);
                typeBuilder.AddInterfaceImplementation(typeOfT);
                typeBuilder.AddInterfaceImplementation(interfaceType);

                //create our _isDirty field, which implements IProxy
                var setIsDirtyMethod = CreateIsDirtyProperty(typeBuilder);

                // Generate a field for each property, which implements the T
                foreach (var property in typeof(T).GetProperties())
                {
                    var isId = property.GetCustomAttributes(true).Any(a => a is KeyAttribute);
                    CreateProperty<T>(typeBuilder, property.Name, property.PropertyType, setIsDirtyMethod, isId);
                }

                var generatedType = typeBuilder.CreateType();

                //assemblyBuilder.Save(name + ".dll");  //NOTE: to save, uncomment

                var generatedObject = Activator.CreateInstance(generatedType);

                TypeCache.Add(typeOfT, generatedObject);
                return (T)generatedObject;
            }


            private static MethodInfo CreateIsDirtyProperty(TypeBuilder typeBuilder)
            {
                var propType = typeof(bool);
                var field = typeBuilder.DefineField("_" + "IsDirty", propType, FieldAttributes.Private);
                var property = typeBuilder.DefineProperty("IsDirty",
                                               System.Reflection.PropertyAttributes.None,
                                               propType,
                                               new Type[] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.SpecialName |
                                                    MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + "IsDirty",
                                             getSetAttr,
                                             propType,
                                             Type.EmptyTypes);
                var currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);
                var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + "IsDirty",
                                             getSetAttr,
                                             null,
                                             new Type[] { propType });
                var currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ret);

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                var getMethod = typeof(Dapper.SqlMapperExtensions.IProxy).GetMethod("get_" + "IsDirty");
                var setMethod = typeof(Dapper.SqlMapperExtensions.IProxy).GetMethod("set_" + "IsDirty");
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);

                return currSetPropMthdBldr;
            }

            private static void CreateProperty<T>(TypeBuilder typeBuilder, string propertyName, Type propType, MethodInfo setIsDirtyMethod, bool isIdentity)
            {
                //Define the field and the property 
                var field = typeBuilder.DefineField("_" + propertyName, propType, FieldAttributes.Private);
                var property = typeBuilder.DefineProperty(propertyName,
                                               System.Reflection.PropertyAttributes.None,
                                               propType,
                                               new Type[] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.Virtual |
                                                    MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
                                             getSetAttr,
                                             propType,
                                             Type.EmptyTypes);

                var currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);

                var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                                             getSetAttr,
                                             null,
                                             new Type[] { propType });

                //store value in private field and set the isdirty flag
                var currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldc_I4_1);
                currSetIL.Emit(OpCodes.Call, setIsDirtyMethod);
                currSetIL.Emit(OpCodes.Ret);

                //TODO: Should copy all attributes defined by the interface?
                if (isIdentity)
                {
                    var keyAttribute = typeof(KeyAttribute);
                    var myConstructorInfo = keyAttribute.GetConstructor(new Type[] { });
                    var attributeBuilder = new CustomAttributeBuilder(myConstructorInfo, new object[] { });
                    property.SetCustomAttribute(attributeBuilder);
                }

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                var getMethod = typeof(T).GetMethod("get_" + propertyName);
                var setMethod = typeof(T).GetMethod("set_" + propertyName);
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);
            }
        }
    }

    public class SqlBuilder
    {
        Dictionary<string, Clauses> data = new Dictionary<string, Clauses>();
        int seq;

        class Clause
        {
            public string Sql { get; set; }
            public object Parameters { get; set; }
        }

        class Clauses : List<Clause>
        {
            string joiner;
            string prefix;
            string postfix;

            public Clauses(string joiner, string prefix = "", string postfix = "")
            {
                this.joiner = joiner;
                this.prefix = prefix;
                this.postfix = postfix;
            }

            public string ResolveClauses(DynamicParameters p)
            {
                foreach (var item in this)
                {
                    p.AddDynamicParams(item.Parameters);
                }
                return prefix + string.Join(joiner, this.Select(c => c.Sql)) + postfix;
            }
        }

        public class Template
        {
            readonly string sql;
            readonly SqlBuilder builder;
            readonly object initParams;
            int dataSeq = -1; // Unresolved

            public Template(SqlBuilder builder, string sql, dynamic parameters)
            {
                this.initParams = parameters;
                this.sql = sql;
                this.builder = builder;
            }

            static System.Text.RegularExpressions.Regex regex =
                new System.Text.RegularExpressions.Regex(@"\/\*\*.+\*\*\/", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);

            void ResolveSql()
            {
                if (dataSeq != builder.seq)
                {
                    DynamicParameters p = new DynamicParameters(initParams);

                    rawSql = sql;

                    foreach (var pair in builder.data)
                    {
                        rawSql = rawSql.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
                    }
                    parameters = p;

                    // replace all that is left with empty
                    rawSql = regex.Replace(rawSql, "");

                    dataSeq = builder.seq;
                }
            }

            string rawSql;
            object parameters;

            public string RawSql { get { ResolveSql(); return rawSql; } }
            public object Parameters { get { ResolveSql(); return parameters; } }
        }


        public SqlBuilder()
        {
        }

        public Template AddTemplate(string sql, dynamic parameters = null)
        {
            return new Template(this, sql, parameters);
        }

        void AddClause(string name, string sql, object parameters, string joiner, string prefix = "", string postfix = "")
        {
            Clauses clauses;
            if (!data.TryGetValue(name, out clauses))
            {
                clauses = new Clauses(joiner, prefix, postfix);
                data[name] = clauses;
            }
            clauses.Add(new Clause { Sql = sql, Parameters = parameters });
            seq++;
        }


        public SqlBuilder LeftJoin(string sql, dynamic parameters = null)
        {
            AddClause("leftjoin", sql, parameters, joiner: "\nLEFT JOIN ", prefix: "\nLEFT JOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder Where(string sql, dynamic parameters = null)
        {
            AddClause("where", sql, parameters, " AND ", prefix: "WHERE ", postfix: "\n");
            return this;
        }

        public SqlBuilder OrderBy(string sql, dynamic parameters = null)
        {
            AddClause("orderby", sql, parameters, " , ", prefix: "ORDER BY ", postfix: "\n");
            return this;
        }

        public SqlBuilder Select(string sql, dynamic parameters = null)
        {
            AddClause("select", sql, parameters, " , ", prefix: "", postfix: "\n");
            return this;
        }

        public SqlBuilder AddParameters(dynamic parameters)
        {
            AddClause("--parameters", "", parameters, "");
            return this;
        }

        public SqlBuilder Join(string sql, dynamic parameters = null)
        {
            AddClause("join", sql, parameters, joiner: "\nJOIN ", prefix: "\nJOIN ", postfix: "\n");
            return this;
        }
    }
}
