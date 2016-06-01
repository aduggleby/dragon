ChainedIdentity
===============

An ASP.NET Identity provider that chains multiple other Identity providers. Modifications are forwarded to all providers, while read operations are performed using the first provider only.
This is useful to use a caching provider together with another provider.


Compatible providers
--------------------

* Dragon.Identity
