SecurityServer
==============

A federation identity provider based on OWIN that uses a chain of Security Token Services:

* AccountSTS: manages external federation information
* ProfileSTS: manages information about the user
* PermissionSTS: manages user permissions

Each service offers an API for which a client is offered. The API is secured using HMAC.


Requirements
------------

* Visual Studio 2015
* SQL Server 2014
* Redis 3.0


Setup
-----

For each STS service:

* Generate a signing certificate
    * Create certificate [2]: New-SelfSignedCertificateEx -Subject "[X500 distinguished name, e.g. CN=Test Cert, OU=Sandbox]" -Exportable
    * Export certificate using certmgr.msc (Personal/Certificates) as PFX include private key, and set the SigningCertificatePassword in the appSettings  (see below) to the private key password
    * Place the certificate somewhere readable by the web server, and set SigningCertificateName in the appSettings (see below) to its path
* Generate a encryption certificate
    * Use the public key of the encryption certificate of the application that uses the SecurityServer (see the previous step, but export without private key as CER)   
* Configure federation (see appSettings in web.config)

        Mandatory app settings:
        <!-- Address of this STS -->
        <add key="SecurityTokenServiceEndpointUrl" value="http://localhost:51387" />
        <!-- Federation Realm -->
        <add key="WtRealm" value="http://WSFedTest/" />
        <!-- Address of the next STS in the chain -->
        <add key="WsFederationEndpointUrl" value="http://thispc.com:51385" />
        <!-- Address of the root STS, i.e. the last STS in the chain (e.g. Account STS) -->
        <add key="ValidIssuer" value="http://thispc.com:51385" />
        <!-- Federation metadata, address of this STS -->
        <add key="FederationHost" value="http://localhost:51387" />
        <!-- Federation metadata, address of this STS -->
        <add key="FederationEndpoint" value="http://localhost:51387" />

        Optional app settings:
        <!-- Signing certificate filename -->
        <add key="SigningCertificateName" value="securityserver.pfx" />
        <!-- Signing certificate password -->
        <add key="SigningCertificatePassword" value="" />
        <!-- Token encryption certificate filename (public key) -->
        <add key="EncryptingCertificateName" value="demo.cer" />
        <!-- Identification of the login provider -->
        <add key="LoginProviderName" value="Dragon" />
        <!-- Federation metadata -->
        <add key="FederationNamespace" value="http://docs.oasis-open.org/wsfed/federation/200706" />

* Initialize the database
    * Use the connection string "Dragon"
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

* Follow the README's in the respective projects for additional specific insructions


Usage
-----

An example web application that uses the SecurityServer for authentication can be found in the Demo project.

To integrate the SecurityServer:
* Configure Windows Identity Foundation (see system.identityModel in the Web.config of the Demo project).
* Specify the service for which the user should be authenticated: The Service ID needs to be added to all federation requests (see Demo.CustomAuthenticationModule and Demo.Controllers.HomeController::SignIn for custom requests).


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
[2] https://gallery.technet.microsoft.com/scriptcenter/Self-signed-certificate-5920a7c6