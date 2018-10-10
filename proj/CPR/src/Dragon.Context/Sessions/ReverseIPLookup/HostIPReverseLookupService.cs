using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Dragon.Context.ReverseIPLookup;

namespace Dragon.Context.Sessions.ReverseIPLookup
{
    // Inspiration from: http://www.hanselman.com/blog/TheWeeklySourceCode37GeolocationGeotargetingReverseIPAddressLookupInASPNETMVCMadeEasy.aspx
    public class HostIpReverseLookupService : IReverseIPLookupService
    {
        private readonly XNamespace NS_GML = (XNamespace)"http://www.opengis.net/gml";
        private readonly XNamespace NS_HOSTIP = (XNamespace)"http://www.hostip.info/api";

        private const string HOSTIPREQUESTURL = "http://api.hostip.info/?ip={0}&position=true";

        public string GetLocationString(string ipAddress)
        {
            if (ipAddress == null) return null;
            var ipAddressObj = IPAddress.Parse(ipAddress);

            if (IPAddress.IsLoopback(ipAddressObj))
                return "Local";

            var ip = ipAddressObj.ToString();
            
            string r;
            using (var w = new WebClient())
            {
                r = w.DownloadString(String.Format(HOSTIPREQUESTURL, ip));
            }

            var xmlResponse = XDocument.Parse(r);

            try
            {
                return xmlResponse.Descendants(WithGLMNS("Hostip"))
                                  .Select(x => string.Format("{0}, {1}",
                                    x.Element(WithGLMNS("name")).Value,
                                    x.Element(WithHostIPNS("countryName")).Value)
                                    ).FirstOrDefault();
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        private XName WithHostIPNS(string node)
        {
            return NS_HOSTIP + node;
        }

        private XName WithGLMNS(string node)
        {
            return NS_GML + node;
        }

       
    }
}
