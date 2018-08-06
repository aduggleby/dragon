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
        <add key="AuthenticationProvider.Microsoft.ClientSecret" value="00000000000000000000000000000000" />
        ...

    * Microsoft authentication service: use [AccountSTS-BaseURL]/signin-microsoft as Redirect URL


Usage
-----

### App groups

App groups are used to restrict access to specific apps in predefined sets of apps.

Automated registration of users to apps is restricted to one app per group, but via the database (IdentityConsumerUser table) multiple apps of the same group can be assigned to users. In this case a selection screen is shown on login, which uses the Url and Name columns in the ConsumerInfo table. The ConsumerInfo table also allows assigning apps to groups. Be sure to use a return url that is restricted to logged in users, so that the app immediately logs in the user.

### Authentication providers

* Google: https://console.cloud.google.com/apis/credentials/
* Microsoft: https://apps.dev.microsoft.com
* Facebook: https://developers.facebook.com/apps
* Twitter: https://apps.twitter.com
* WsFederation

#### WsFederation

All providers prefixed with "WsFederation-" will be added as WS-Federation provider, and as such require Wtrealm and MetadataAddress configuration.
For an example, see [Web.config.default](Web.config.default).
