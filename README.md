# Dragon Web Framework
A compilation of modules for faster web application development for ASP.NET.

Currently there are modules for E-Mail sending, Data Layer (based on Dapper) and Security Modules (HMAC Authentication for both Client and Server, Security Server).

Modules go through the following cycle:

- **Development**: Use is not recommended, module may not survive. 
- **Usable**: Used in production. Goal is to stabilize the API. Module may stay in this phase for some time while it starts being used in multiple production projects.
- **Stable**: No major flaws or API issues found while being used in multiple projects. Except API not to change. Can be used for production systems (via Nuget) but often lack in documentation. 
- **Released**: Reviewed documentation has been published.


All modules are licensed under [MIT](https://opensource.org/licenses/MIT) unless otherwise specified in the module itself.





## [Dragon.Mail](https://github.com/aduggleby/dragon/tree/master/proj/Mail)

Status: Released, Version: [1.8.1](http://www.nuget.org/packages/Dragon.Mail/)

The Mail Templating and Sending Module (part of Dragon Web Framework) for .NET that supports 

- Handlebar Templates (HTML & plain-text), 
- Internationalization, 
- Summary E-Mails (Batching, i.e. do not send more than 1 email per X hours) and 
- Asynchronous Sending.





## [Dragon.Security.Hmac](https://github.com/aduggleby/dragon/tree/master/proj/Security.Hmac)

Comprised of the following releases:

- ASP.NET Authentication Module - Status: Released, Version: [1.2.1](http://www.nuget.org/packages/Dragon.Security.Hmac.Module/)
- Core Library and Client - Status: Released, Version: [1.2.0](http://www.nuget.org/packages/Dragon.Security.Hmac.Core/)
- Management REST API and Web Application - Status: In Development, Version: [1.0.1](http://www.nuget.org/packages/Dragon.Security.Hmac.ManagementWeb/)

The aim of the Hmac module is to allow quickly setting up HMAC-based authorization 
for requests from client applications (apps) to web services (services).



## [Dragon.Data](https://github.com/aduggleby/dragon/tree/master/proj/Data)

Status: Stable, Version: [1.4.0](http://www.nuget.org/packages/Dragon.Data/)

Add-on library for [Dapper](https://github.com/StackExchange/dapper-dot-net) implementing:

- Repository pattern 
- Extension methods for IDbRepository
- SQL Builder for typed queries
- Metadata Utilities
- Table (Schema) Generator





## [Dragon.Files](https://github.com/aduggleby/dragon/tree/master/proj/Files)

Comprised of the following releases:

- Core Library (with local file system module) - Status: Released, Version: [1.2.0](http://www.nuget.org/packages/Dragon.Files/)
- Azure Blob Storage Module - Status: Usable, Version: [1.0.2](http://www.nuget.org/packages/Dragon.Files.AzureBlobStorage/) 
- Amazon S3 Module - Status: Usable, Version: [1.2.1](http://www.nuget.org/packages/Dragon.Files.S3/)
- ASP.NET MVC extensions - Status: Usable, Version [1.2.1](http://www.nuget.org/packages/Dragon.Files.MVC/)

Abstracts file storage for a number of file storage systems (local filesystem, Azure blob storage and S3) and allow storing and retrieving files by resource ID. 




## [Dragon.SecurityServer](https://github.com/aduggleby/dragon/tree/master/proj/SecurityServer)

Status: Stable (No NuGet packages)

A federation identity provider based on ASP.NET Identity/OWIN that uses a chain of Security Token Services:

- AccountSTS: manages external federation information
- ProfileSTS: manages information about the user
- PermissionSTS: manages user permissions

Each service offers an API for which a client is offered. The API is secured using HMAC.




## [Dragon.Diagnostics](https://github.com/aduggleby/dragon/tree/master/proj/Diagnostics)

Status: Stable (No NuGet packages)

Utility Framework for creating self-service diagnostics tools that run a set of pre-configured tests, create a log for users to send to your support team.

Currently available tests:

- Ping and TraceRoute
- HTTP and WebSocket Connections
- Client Network Interfaces
- Operating System Information
- Browser Information
- SSL connection and certificate (possibly MITM intermediate proxies)

Also includes a customizable web page that can server as download location and testing target.



## Contributors

[Alex Duggleby](http://dualconsult.com), Joshi