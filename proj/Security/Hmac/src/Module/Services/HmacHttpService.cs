using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.WebPages;
using Dragon.Security.Hmac.Module.Configuration;
using Dragon.Security.Hmac.Module.Services.Validators;
using IValidator = Dragon.Security.Hmac.Module.Services.Validators.IValidator;

namespace Dragon.Security.Hmac.Module.Services
{
    public class HmacHttpService : IHmacHttpService
    {
        public IDictionary<string, IValidator> Validators { get; set; }
        public IDictionary<string, StatusCode> StatusCodes { get; set; }

        private IEnumerable<PathInfo> PathsRegex { get; set; }
        private readonly string _signatureParameterKey;

        public HmacHttpService(IEnumerable<PathConfig> paths, string signatureParameterKey)
        {
            _signatureParameterKey = signatureParameterKey;
            PathsRegex = paths.Select(x => new PathInfo
                {
                    Regex = new Regex(x.Path, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    Type = x.Type == PathConfig.PathType.Include ? PathInfo.PathType.Include : PathInfo.PathType.Exclude
                });
        }

        public StatusCode IsRequestAuthorized(string rawUrl, NameValueCollection queryString)
        {
            var matchingPath = PathsRegex.FirstOrDefault(x => x.Regex.IsMatch(rawUrl));
            if (matchingPath != null && matchingPath.Type == PathInfo.PathType.Exclude)
            {
                return StatusCode.Authorized;
            }
            if (IsMandatoryParameterMissingOrEmpty(Validators.Where(x => !x.Value.IsOptional()).Select(x => x.Key).ToArray(), queryString))
            {
                return StatusCode.ParameterMissing;
            }

            ((HmacValidator)Validators[_signatureParameterKey]).SetQueryString(queryString);

            foreach (var parameter in Validators.Keys)
            {
                if (!Validators[parameter].Parse(queryString.Get(parameter)) && !Validators[parameter].IsOptional())
                {
                    return StatusCodes[parameter];
                }
            }
            foreach (var parameter in Validators.Keys)
            {
                if (!Validators[parameter].Validate())
                {
                    return StatusCodes[parameter];
                }
            }
            foreach (var parameter in Validators.Keys)
            {
                Validators[parameter].OnSuccess();
            }

            return StatusCode.Authorized;
        }

        private static bool IsMandatoryParameterMissingOrEmpty(string[] requiredKeys, NameValueCollection queryString)
        {
            return requiredKeys.Except(queryString.Keys.Cast<string>()).Any() ||
                requiredKeys.Select(queryString.Get).Any(x => x == null || x.IsEmpty());
        }
    }
}