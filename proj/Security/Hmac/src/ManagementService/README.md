Dragon.Security.Hmac.ManagementService
======================================

This component provides a RESTful API for managing users and apps of the Dragon.Security.Hmac module.


Requirements
------------

* Visual Studio 2013
* SQL Server 2014


Setup
-----

* Create a database and adapt the Web.config as described in the Solution's README.md file.


Usage
-----

* Get all:
  * GET api/[App|User]
* Get:
  * GET api/[App|User]/[id]
* Add:
  * POST api/[App|User]
* Edit:
  * PUT api/[App|User]
* Delete:
  * DELETE api/[App|User]/[id]

See tests for details.
