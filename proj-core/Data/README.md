
# Dragon.Data

An repository pattern wrapper around [Dapper ORM](https://github.com/StackExchange/Dapper).

- Repository class (generates SQL for Insert, Select, Update)
- Metadata fetchers for SQL tables
- SQL Expression Builder from Linq (Example: `x.Where<TestClass>().Group(g => g.Like(x => x.A, "Alice").And().SmallerThan(x => x.B, 2)`);

Available in netstandard1.6 and netstandard2.0.

License: [MIT](https://opensource.org/licenses/MIT)

**Table of contents**
* [Architecture](#architecture)
* [Downloading](#downloading)
* [Basic Usage (Synchronous)](#basic-usage--synchronous-)
  + [Loading templates](#loading-templates)
  + [Application Configuration](#application-configuration)
  + [Basic e-mail sending](#basic-e-mail-sending)
* [Asynchronous sending and batching](#asynchronous-sending-and-batching)
  + [Extending templates for batching](#extending-templates-for-batching)
  + [Application configuration](#application-configuration)
  + [Database setup and configuration](#database-setup-and-configuration)
  + [Service setup](#service-setup)
  + [Application example](#application-example)
* [Advanced Use Cases, Extensions and Customization](#advanced-use-cases--extensions-and-customization)
* [Used By](#used-by)

## Downloading

Either build from source code using Visual Studio >= 2017 or find everything except the Windows Service (required only for async sending) in the current NuGet (> 2.0.0 contains .NET Core libraries) packages at:

http://www.nuget.org/packages/Dragon.Data/

## Basic Usage 

Dragon.Data requires some configuration, a setup of the dependency injection and finally you can inject repository interfaces into your classes.

### Configuration
In your `appsettings.config` add the connection string. 

    "dragon": {
        "data": {
          "connectionString": "Server=localhost;Database=<db>;Trusted_Connection=True;"
        }
      }

The `DefaultDbConnectionContextFactory` will use this value to connect, but you can override the `CreateConnection` method there to implement a custom method for getting the connection string.

### Setup

In your Startup.cs or Program.cs (wherever you setup your dependency injection container) add the following:

    services.AddSingleton<IDbConnectionContextFactory, DefaultDbConnectionContextFactory>();
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
	services.AddScoped<RepositorySetup, RepositorySetup>();

### Injecting

In your classes you can now inject `IRepository<X>` where X is a class that represents your table structure.

    public class MyTable
    {
          public Guid ID { get; set; }
          public string Email { get; set; }
          public string UserName { get; set; }
          public DateTime? CreatedOn { get; set; }
    }

Data types must correspond to SQL data types. Nullable types are supported.

Optional attributes are:

**Class attributes:** 
Specifying the table name (otherwise the class name is used):

    [Table("AspNetUsers")]
    public class MyTable
    
Specifying the database schema to use (otherwise the 'dbo' is used):

    [Schema("schemaname")]
    public class MyTable

**Property attributes:**

Specifying the column name (otherwise the property name is used):

    [Column("user_id")]
    public Guid ID { get; set; }
          
Specify the property is a primary key (or part of a primary key):

    [Key("")]
    public string Email { get; set; }

Specifies that the property is ignored when generating SQL (i.e. a property without a table backing):

    [NoColumn("")]
    public string Email { get; set; }

Specifies the maximum length of the column when generating a table using RepositorySetup (or using SQLBuilder to generate a CREATE statement):

    [Length(200)]
    public string Description { get; set; }
 
	[Length("MAX")]
    public string Description { get; set; }

### Repository methods

The following information is also provided via Intellisense

    /// <summary>
    /// Get the record by primary key.
    /// T must have a single property marked with KeyAttribute.
    /// </summary>
    /// <param name="pk"></param>
    /// <returns></returns>
    T Get(dynamic pk);

    /// <summary>
    /// Get the record by primary key.
    /// T must have a single property marked with KeyAttribute.
    /// </summary>
    /// <param name="keyModel"></param>
    /// <returns></returns>
    T Get(T keyModel);

    /// <summary>
    /// Gets a set of records by their primary keys.
    /// T must have a single property marked with KeyAttribute.
    /// </summary>
    /// <param name="pks"></param>
    /// <returns></returns>
    IEnumerable<T> Get(IEnumerable<object> pks);
    
    /// <summary>
    /// Gets all records from the table.
    /// </summary>
    /// <returns></returns>
    IEnumerable<T> GetAll();

    /// <summary>
    /// Executes an arbitary parametrized SQL query ({TABLE} token will be replaced by the repositories table) and returns the results set.
    /// </summary>
    /// <param name="sql">A parametrized SQL Query (i.e. SELECT {cols} FROM {TABLE} WHERE COL1 = @col1value)</param>
    /// <param name="param">The parameter values (i.e. new {col1value="abc"})</param>
    /// <returns></returns>
    IEnumerable<T> Query(string sql, dynamic param = null);

    /// <summary>
    /// Executes an arbitary parametrized SQL query but projects into a view ({TABLE} token will be replaced by the repositories table) and returns the results set.
    /// </summary>
    /// <typeparam name="TView">The view class to project into</typeparam>
    /// <param name="sql">A parametrized SQL Query (i.e. SELECT {cols} FROM {TABLE} WHERE COL1 = @col1value)</param>
    /// <param name="param">The parameter values (i.e. new {col1value="abc"})</param>
    /// <returns></returns>
    IEnumerable<TView> QueryView<TView>(string sql, dynamic param = null) where TView : class;

    /// <summary>
    /// Executes an arbitary parametrized SQL query ({TABLE} token will be replaced by the repositories table) and returns the first record value. Used for scalar queries such as "SELECT COUNT(1) FROM {TABLE}".
    /// </summary>
    /// <typeparam name="TReturn">The data type to convert the scalar return value to.</typeparam>        
    /// <param name="sql">A parametrized SQL Query (i.e. SELECT {cols} FROM {TABLE} WHERE COL1 = @col1value)</param>
    /// <param name="param">The parameter values (i.e. new {col1value="abc"})</param>
    /// <returns></returns>
    TReturn ExecuteScalar<TReturn>(string sql, dynamic param = null);
    
    /// <summary>
    /// Executes an arbitary parametrized SQL query ({TABLE} token will be replaced by the generic type TDBObject) and returns the first record value. Used for scalar queries such as "SELECT COUNT(1) FROM {TABLE}".
    /// </summary>
    /// <typeparam name="TReturn">The data type to convert the scalar return value to.</typeparam>
    /// <typeparam name="TDBObject">The data type used to transform the query (in case you are not running a query against the repository generic type T.</typeparam>
    /// <param name="sql">A parametrized SQL Query (i.e. SELECT {cols} FROM {TABLE} WHERE COL1 = @col1value)</param>
    /// <param name="param">The parameter values (i.e. new {col1value="abc"})</param>
    /// <returns></returns>
    TReturn ExecuteScalar<TReturn, TDBObject>(string sql, dynamic param = null);
    
    /// <summary>
    /// Executes an arbitary parametrized SQL command without a result ({TABLE} token will be replaced by the repositories table).
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    void Execute(string sql, dynamic param = null);

    void ExecuteSP(string sql, dynamic param = null);

    /// <summary>
    /// Executes an arbitary parametrized SQL query ({TABLE} token will be replaced by the generic type TDBObject)
    /// and splits the returned columns into two objects (split occurs at the primary key of the second table). 
    /// See "Multi Mapping" in https://code.google.com/p/dapper-dot-net/
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sql"></param>
    /// <param name="mapping">See "Multi Mapping" in https://code.google.com/p/dapper-dot-net/</param>
    /// <param name="param"></param>
    /// <returns></returns>
    IEnumerable<TResult> Query<TFirst, TSecond, TResult>(string sql, Func<TFirst, TSecond, TResult> mapping,
        dynamic param = null) where TResult : class;
  
    /// <summary>
    /// Inserts a record and expects either database to create key (auto incrementing keys) or a key value to be supplied (e.g. for GUID keys).
    /// Will return the primary key.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key used.</typeparam>
    /// <param name="obj">The record to insert.</param>
    /// <returns></returns>
    TKey Insert<TKey>(T obj);

    /// <summary>
    /// Inserts a record and expects either database to create key (auto incrementing keys) or a key value to be supplied (e.g. for GUID keys).
    /// </summary>
    /// <param name="obj">The record to insert.</param>
    /// <returns></returns>
    void Insert(T obj);

    /// <summary>
    /// Inserts or updates a record depending on wether the key has been set. Will attempt to set the primary key before inserting (only valid for GUID type primary keys).
    /// T must have a single property marked with KeyAttribute.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    TKey Save<TKey>(T obj);

    /// <summary>
    /// Updates a record and requires keys to be set.
    /// </summary>
    /// <param name="obj">The record to update.</param>
    void Update(T obj);

    /// <summary>
    /// Deletes a record and requires keys to be set.
    /// </summary>
    /// <param name="obj">The record to update.</param>
    void Delete(T obj);

    /// <summary>
    /// Performs a query with a set of AND filters.
    /// </summary>
    /// <param name="where">The values to filter for. Each entry will be concatenated with AND.</param>
    /// <returns></returns>
    IEnumerable<T> GetByWhere(Dictionary<string, object> @where);

## Direct IDbConnection access and transactions

If you inject IDbConnectionContextFactory you can gain access to a the IDbConnection object (and thus Dapper directly):

	public class YourService(IDbConnectionContextFactory ctx) {...}

    public void YourMethod()
    {
        await ctx.InDatabase(async (db, tx) =>
        {
	        // db is IDbConnection
	        // tx is the current transaction
            await db.X();
        });
    }
    
For transactions you use `TransactionDbConnectionContextFactory`. This class will create a transaction when the context factory class is instantiated and commit the transaction when disposed. It is recommend to use the `using` syntax.

	public class YourService(
		Microsoft.Extensions.Configuration.IConfiguration confix,
		Microsoft.Extensions.Logging.ILoggerFactory loggerFactory,
	) {...}

    using (var sql = new TransactionDbConnectionContextFactory(config, loggerFactory))
    {
        await sql.InDatabase(async (db, tx) =>
        {
            await db.X();
        });
		
		// do something else
		
		await sql.InDatabase(async (db, tx) =>
        {
	        // tx is the same transaction as above
            await db.X();
        });
    }
        
### Table Setup

The EnsureExist method on IRepositorySetup will check if the table exists and otherwise create it. Requires the connection string user to have CREATE permissions.

	public class YourService(IRepositorySetup setup) {...}

    public void YourMethod()
    {
        setup.EnsureExists<TableClass>();
	}

### Metadata

Use the TableMetadata class if you want to enumerate the properties on a table class or property (this will not read the database table schema, just the class properties and attributes):

    var metadataTable = new TableMetadata();
    MetadataHelper.MetadataForClass(typeof(MyTable), ref metadataTable);
    
    var metadataProp = new PropertyMetadata();

    MetadataHelper.MetadataForProperty(
        typeof(TestClass).GetProperty("x"),
        ref metadataProp);

Table metadata provides:

    public class TableMetadata
    {
        public string ClassName { get; set; }
    
        public string TableName { get; set; }
        public string Schema { get; set; }
    
        public List<PropertyMetadata> Properties { get; set; }
    
        public TableMetadata()
        {
            Properties = new List<PropertyMetadata>();
        }
    }
    
Property metadata provides:

    public class PropertyMetadata
    {
        public bool IsPK { get; set; }
        public bool IsOnlyPK { get; set; }
        public bool IsAutoIncrementingPK { get; set; }

        public bool IsTimestamp { get; set; }

        public bool Indexed { get; set; }
        public string PropertyName { get; set; }
        public string ColumnName { get; set; }
        public string Length { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public string SqlTypeString { get; set; }
        public string SqlKeyTypeString { get; set; }
        public bool Nullable { get; set; }

        public Action<PropertyMetadata, TableMetadata> AfterPropertyMetadataSet { get;set;}
    }

### SQL Generator

The static class TSQLGenerator will generate a number of SQL statements based on the metadata you pass.

    var md = new TableMetadata();
    MetadataHelper.MetadataForClass(typeof(MyTable), ref md);
    
    var sql = TSQLGenerator.BuildCreate(md, true);

### SQL Where Generator

If you want to construct a SQL Where statement using a LINQ syntax you can use the Where class:

    var expr = new Where<TestClass>().IsEqual(x => x.A, "Bob")
                   .And(g => g.Like(x => x.B, "Alice").Or().SmallerThan(x => x.C, 2))
                   .And(g => g.GreaterThanOrEqualTo(x => x.C, 3).Or().IsEqual(x => x.D, "Chris"));

    public class TestClass
    {
        [Column("AA")]
        public string A { get; set; }

        public string B { get; set; }
        
        [Column("CC")]
        public string C { get; set; }
        
        public string D { get; set; }
    }    
    
 To generate the SQL with a provided parameter dictionary use the following statement:
 
	var param = new Dictionary<string, object>();
	var sql = expr.Build(param).ToString();
   
The resulting statement is:
   
    WHERE [AA]=@A AND ([B] LIKE @B OR [CC]<@C) AND ([CC]>=@C2 OR [D]=@D)

# Used By

- [DealMatrix Innovation Scouting Platform](http://www.dealmatrix.com)
- [WhatAVenture Innovation Platform](http://www.whataventure.com)
- [FoundersExperts](http://foundersexperts.com)
- [DualConsult](http://dualconsult.com)
- [EasySlides](http://www.easyslides.co)
- [Austrian Energy Agency](http://www.monitoringstelle.at)

