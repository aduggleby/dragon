using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;

namespace Dragon.SecurityServer.Common.Handlers
{
    /// <summary>
    /// See <a href="https://syfuhs.net/2010/11/03/generating-federation-metadata-dynamically/">this link</a> for more information.
    /// </summary>
    public class FederationMetadataHandler : IHttpHandler
    {
        public string Host { get; set; }
        public string Namespace { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public string Endpoint { get; set; }

        private EntityDescriptor _entity;

        public FederationMetadataHandler()
        {
            InitializeFromConfig();
        }

        private void InitializeFromConfig()
        {
            SigningCredentials = SecurityHelper.CreateSignupCredentialsFromConfig();
            Host = WebConfigurationManager.AppSettings["FederationHost"];
            Namespace = WebConfigurationManager.AppSettings["FederationNamespace"];
            Endpoint = WebConfigurationManager.AppSettings["FederationEndpoint"];
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ClearHeaders();
            context.Response.Clear();
            context.Response.ContentType = "text/xml";
            SerializeMetadata(context.Response.OutputStream);
        }

        public void SerializeMetadata(Stream stream)
        {
            var serializer = new MetadataSerializer();
            serializer.WriteMetadata(stream, GenerateEntities());
        }

        private EntityDescriptor GenerateEntities()
        {
            if (_entity != null)
                return _entity;

            var sts = new SecurityTokenServiceDescriptor();
            //FillOfferedClaimTypes(sts.ClaimTypesOffered);
            FillEndpoints(sts);
            FillSupportedProtocols(sts);
            FillSigningKey(sts);

            _entity = new EntityDescriptor(new EntityId(Host))
            {
                SigningCredentials = SigningCredentials
            };

            _entity.RoleDescriptors.Add(sts);
            return _entity;
        }

        private void FillSigningKey(RoleDescriptor sts)
        {
            var signingKey = new KeyDescriptor(SigningCredentials.SigningKeyIdentifier)
            {
                Use = KeyType.Signing
            };

            sts.Keys.Add(signingKey);
        }

        private void FillSupportedProtocols(RoleDescriptor sts)
        {
            sts.ProtocolsSupported.Add(new System.Uri(Namespace));
        }

        private void FillEndpoints(SecurityTokenServiceDescriptor sts)
        {
            if (string.IsNullOrEmpty(Endpoint)) return;

            var endpoint = new EndpointReference(string.Format("{0}", Endpoint));

            sts.SecurityTokenServiceEndpoints.Add(endpoint);
            sts.PassiveRequestorEndpoints.Add(endpoint);
        }

        private static void FillOfferedClaimTypes(ICollection<DisplayClaim> claimTypes)
        {
            claimTypes.Add(new DisplayClaim(ClaimTypes.Name, "Name", ""));
            claimTypes.Add(new DisplayClaim(ClaimTypes.Email, "Email", ""));
            claimTypes.Add(new DisplayClaim(ClaimTypes.Role, "Role", ""));
        }

        public bool IsReusable { get { return false; } }
    }
}