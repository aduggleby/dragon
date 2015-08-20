using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.WebPages;
using Dragon.Security.Hmac.Core.Service;
using Dragon.Security.Hmac.Module.Configuration;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;

namespace Dragon.Security.Hmac.Module.Services
{
    public class HmacHttpService : IHmacHttpService
    {
        public IHmacService HmacService { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IAppRepository AppRepository { get; set; }

        private IEnumerable<PathInfo> PathsRegex { get; set; }

        private readonly string _serviceId;
        private readonly string _signatureParameterKey;

        public HmacHttpService(string serviceId, IEnumerable<PathConfig> paths, string signatureParameterKey)
        {
            _serviceId = serviceId;
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

            var mandatoryParameterNames = new[] { "appid", "serviceid", "userid", "expiry", _signatureParameterKey };
            if (IsMandatoryParameterMissingOrEmpty(mandatoryParameterNames, queryString))
            {
                return StatusCode.ParameterMissing;
            }

            var guidParsingSuceeded = true;
            Guid appId;
            guidParsingSuceeded &= Guid.TryParse(queryString.Get("appid"), out appId);
            Guid userId;
            guidParsingSuceeded &= Guid.TryParse(queryString.Get("userid"), out userId);
            Guid serviceId;
            guidParsingSuceeded &= Guid.TryParse(queryString.Get("serviceid"), out serviceId);
            if (!guidParsingSuceeded)
            {
                return StatusCode.InvalidParameterFormat;
            }

            var signature = queryString.Get(_signatureParameterKey);

            if (!IsExpiryValid(queryString.Get("expiry")))
            {
                return StatusCode.InvalidExpiryOrExpired;
            }

            if (!serviceId.ToString().Equals(_serviceId, StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode.InvalidOrDisabledServiceId;
            }

            var app = AppRepository.Get(appId, serviceId);
            if (app == null || !app.Enabled)
            {
                return StatusCode.InvalidOrDisabledAppId;
            }

            var actual = HmacService.CalculateHash(HmacService.CreateSortedQueryString(queryString), app.Secret);
            if (actual.ToLower() != signature.ToLower())
            {
                return StatusCode.InvalidSignature;
            }

            var user = UserRepository.Get(userId, serviceId);
            if (user == null)
            {
                UserRepository.Insert(new UserModel
                {
                    AppId = appId, 
                    ServiceId = serviceId, 
                    Enabled = true, 
                    UserId = userId, 
                    CreatedAt = DateTime.UtcNow
                });
            }
            if (user != null && !user.Enabled)
            {
                return StatusCode.InvalidOrDisabledUserId;
            }

            return StatusCode.Authorized;
        }

        private static bool IsExpiryValid(string expiryString)
        {
            try
            {
                var expiry = long.Parse(expiryString);
                if (expiry > DateTime.UtcNow.Ticks)
                {
                    return true;
                }
            }
            catch (FormatException)
            {
                return false;
            }
            return false;
        }

        private static bool IsMandatoryParameterMissingOrEmpty(string[] requiredKeys, NameValueCollection queryString)
        {
            return requiredKeys.Except(queryString.Keys.Cast<string>()).Any() ||
                requiredKeys.Select(queryString.Get).Any(x => x == null || x.IsEmpty());
        }
    }
}