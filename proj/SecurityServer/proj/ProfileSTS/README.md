ProfileSTS
==========

A STS used to store user profile claims.


Requirements
------------

* Visual Studio 2013


Setup
-----

### Development

* Copy Web.config.default to Web.config, adapt connection strings, federation settings, and HMAC settings
* Create a database (run the Client.Test.Initializer tests, default location: App_Data\aspnet-ProfileSTS-Dragon.mdf)
* Configure a vhost for the AccountSTS in IIS Express (http://localhost:51386)


Usage
-----

Use the client to manage user claims.


#### Client compatibility

* Profile v1.0: ProfileSTS.Client v1.0
* Profile v1.1: ProfileSTS.Client v1.1
