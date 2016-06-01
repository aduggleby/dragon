AccountSTS
==========

A STS that uses local user accounts as well as third party login providers.


Requirements
------------

* Visual Studio 2013


Setup
-----

### Development

* Copy Web.config.default to Web.config, adapt connection strings, federation settings, and HMAC settings
* Create a database (run the Client.Test.Initializer tests, default location: App_Data\aspnet-AccountSTS-Dragon.mdf)
* Configure a vhost for the AccountSTS in IIS Express (http://thispc.com:51385)
* Configure the authentication providers:

        <add key="AuthenticationProviders" value="Microsoft, Facebook, Twitter" />
        <add key="AuthenticationProvider.Microsoft.ClientID" value="0000000000000000" />
        <add key="AuthenticationProvider.Microsoft.ClientSecret" value="00000000000000000000000000000000" /

    * Microsoft authentication service: use [AccountSTS-BaseURL]/signin-microsoft as Redirect URL