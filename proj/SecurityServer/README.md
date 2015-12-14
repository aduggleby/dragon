SecurityServer
==============

A federation identity provider based on OWIN that uses a chain of Security Token Services:

* AccountSTS: manages external federation information
* ProfileSTS: manages information about the user
* PermissionSTS: manages user permissions

Each service offers an API for which a client is offered. The API is secured using HMAC.


Requirements
------------

* Visual Studio 2013
* SQL Server 2014
* Redis 3.0


Setup
-----

For each STS service:
* Generate a signing certificate
* Configure federation (see appSettings in web.config)
* Initialize the database
    * Release
        * Adapt the HMAC data (insert into [AppModel] in v1.0.0.0) according to [1]
        * Import the sql/migrations scripts
    * Debug
        * Run the setup controller at /Setup
        * Import the Exceptions table from GenericSTSClient.Test\Resources\CreateExceptionsTable.txt
* Configure HMAC between STS and STS client according to [1]
* Setup a Redis cache (optional, already done in the AccountSTS)
    * Adapt the configuration according to the Identity.Redis.Test, and also set RedisAddress to the host where the Redis server is running
    * Configure the SimpleInjectorInitializer, add as first store to the ChainedIdentity UserStore the following:

            new Identity.Redis.UserStore<AppMember>(new RedisUserStore<Identity.Redis.IdentityUser>(connectionMultiplexer), connectionMultiplexer),


Tests
-----

* Prepare the database:
  Uncomment the Ignore attribute in the Initializer.cs files and run their tests while to services are stopped.

  Warning: This will overwrite the SQL Server data files in App_Data of the respective STS.

* Run the integration tests:
  Ensure that Redis and the services are running.


References
----------

[1] https://github.com/aduggleby/dragon/tree/restructuring/proj/Security/Hmac