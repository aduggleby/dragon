AccountSTS
==========

A STS that uses local user accounts as well as third party login providers.


Requirements
------------

* Visual Studio 2013


Setup
-----

* Copy Web.config.default to Web.config, adapt connection strings, federation settings, and HMAC settings
* Create a database (run the Client.Test.Initializer tests, default location: App_Data\aspnet-AccountSTS-Dragon.mdf)
* Configure a vhost for the AccountSTS in IIS Express (http://thispc.com:51385)
* Microsoft authentication service: use http://thispc.com:51385/signin-microsoft as Redirect URL