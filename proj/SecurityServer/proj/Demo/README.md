Demo
====

An example that uses federation to restrict access. It uses the AccountSTS client / OWIN to allow updating user data.


Setup
-----

* Build
* Configure Federation (Web.config)
  * system.identityModel and system.identityModel.services
  * appSettings: Dragon.Security.Hmac.*, AccountStsUrl, Dragon.SecurityServer.EncryptingCertificateName, Dragon.SecurityServer.EncryptingCertificatePassword
* For local deployment: comment out the securityTokenHandlers in system.identityModel/identityConfiguration