Dragon.Security.Hmac
====================

The aim of the Hmac module is to allow quickly setting up HMAC-based authorization 
for GET requests from client applications (apps) to web services (services).

Following parameter are used:

* appId: A unique id of a client application which consumes a service, each app shares a secret with each service
* serviceId: A unique id of a web service, each service shares a secret with each app
* userId: A unique id of an user of a client application
* expiry: A timestamp in the future which specifies until when the request is valid, represented as ticks
* signature: A hash over all parameter values using the shared secret of the app and the target service

Apps add these parameter to their requests to services. For calculating the signature the Core project can be utilized by apps.
The service checks for each requests in the config if authorization is necessary, and if so gets the shared secret from the database,
and uses it to validate the signature. New users are added to the database for the requested app/service combination.

To grant access to restricted paths, following conditions need to be met:

* The signature is valid
* The user is not disabled
* The expiry is in the future
* The request does not target a different service

This module does not prevent users from spoofing requests, but aims to prevent tampering with request parameter.


Requirements
------------

* Visual Studio 2013
* SQL Server 2014 (SQL Server 2012 Express LocalDB for development)


Setup
-----

For the web application that exposes the service:

* Install the Dragon.Security.Hmac.Module package or build the Module project and copy the resulting binaries to the bin directory of the service.
* Configure the module (Web.config, it is assumed a connection string exists), e.g.:

        <configuration>
          <configSections>
            ...
            <sectionGroup name="dragon">
              <sectionGroup name="security">
                <section name="hmac" type="Dragon.Security.Hmac.Module.Configuration.DragonSecurityHmacSection, Dragon.Security.Hmac.Module" />
              </sectionGroup>
            </sectionGroup>
            ...
          </configSections>
          ...
          <dragon>
            <security>
              <hmac
                serviceId="00000001-0001-0001-0001-000000000001"
                connectionStringName="DefaultConnection"
                usersTableName="Users"
                appsTableName="Apps">
                <Paths>
                  <add name="allowed" path="^/Home/Public/.*$" type="Exclude" />
                  <add name="default" path=".*" type="Include" />
                </Paths>
              </hmac>
            </security>
          </dragon>
          ...
        <configuration>

    where 
    connectionStringName refers to a name of an existing connection string, 
    Paths specifies a lists of regular expressions which define for which paths the authorization is skipped/necessary (type Exclude/Include respectively), defaults to necessary authorization
    The first matching path will be applied.

    As alternative to the config section, app settings can be used, e.g.:


        <add key="Dragon.Security.Hmac.ServiceId" value="00000001-0001-0001-0001-000000000001"/>
        <add key="Dragon.Security.Hmac.ConnectionStringName" value="DefaultConnection"/>
        <add key="Dragon.Security.Hmac.UsersTableName" value="Users"/>
        <add key="Dragon.Security.Hmac.AppsTableName" value="Apps"/>
        <add key="Dragon.Security.Hmac.SignatureParameterKey" value="signature"/>
        <add key="Dragon.Security.Hmac.UseHexEncoding" value="false"/>
        <add key="Dragon.Security.Hmac.PathNames" value="allowed;default"/>
        <add key="Dragon.Security.Hmac.PathTypes" value="Exclude;Include"/>
        <add key="Dragon.Security.Hmac.PathRegexes" value="^/Home/Public/.*$;.*"/>


* Prepare the database, i.e. import the sql/migration scripts.

* Register the services, e.g.

    INSERT INTO [dbo].[Apps] ([AppId], [ServiceId], [Secret], [Enabled], [CreatedAt]) VALUES 
        (N'00000001-0001-0001-0003-000000000001', N'00000001-0001-0001-0001-000000000001', N'secret', 1, N'2015-02-05 00:00:00')


For the client that accesses the service:

* Install the Dragon.Security.Hmac.Core package or reference the Core library.
* Add the appid, userid, expiry, signature parameters to your requests to the service, e.g.

    var queryString = HttpUtility.ParseQueryString(string.Empty);
    // add your parameter...
    queryString["appid"] = "00000001-0001-0001-0003-000000000001";
    queryString["serviceid"] = "00000001-0001-0001-0001-000000000001";
    queryString["userid"] = "00000001-0002-0001-0002-000000000001";
    queryString["expiry"] = DateTime.UtcNow.AddDays(+1).Ticks.ToString();
    var hmacService = new HmacSha256Service();
    queryString["signature"] = hmacService.CalculateHash(hmacService.CreateSortedQueryValuesString(queryString), "secret");
    var hmacQueryString = queryString.ToString();


HowTo
-----

* Customize the signature parameter key:

    In the app set the SignatureParameterKey property on the HmacSha256Service object, 
    in the service set the dragon.security.hmac.signatureParameterKey property.

* Change the signature encoding:

    Default is Base64, to switch to Hex:
    In the app set the UseHexEncoding property on the HmacSha256Service object to true,
    in the service set the dragon.security.hmac.useHexEncoding property to true.


Components
----------

* Core

  A library for apps to generate HMAC signatures for GET requests.

* Module

  A HTTP Module for authorizing HMAC signed GET requests.

* ManagementService

  A REST API which allows managing users and apps.
