Demo
====

An example that uses federation to restrict access. It uses the AccountSTS client / OWIN to allow updating user data.


Setup
-----

* Build
* Configure Federation (Web.config)
  * system.identityModel: To determine the thumbprint for the issuerNameRegistry get the thumbprint from the certificate or debug into System.IdentityModel.Tokens.ConfigurationBasedIssuerNameRegistry::GetIssuerName.
  * system.identityModel.services
  * appSettings: Dragon.Security.Hmac.*, AccountStsUrl, Dragon.SecurityServer.EncryptingCertificateName, Dragon.SecurityServer.EncryptingCertificatePassword
* For local deployment: comment out the securityTokenHandlers in system.identityModel/identityConfiguration