using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        private IEnumerable<PathInfo> PathInfos { get; set; }

        private readonly string _serviceId;
        private readonly string _signatureParameterKey;

        public HmacHttpService(string serviceId, IEnumerable<PathConfig> paths, string signatureParameterKey)
        {
            _serviceId = serviceId;
            _signatureParameterKey = signatureParameterKey;
            PathInfos = paths.Select(x => new PathInfo
                {
                    Regex = new Regex(x.Path, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    Type = x.Type == PathConfig.PathType.Include ? PathInfo.PathType.Include : PathInfo.PathType.Exclude,
                    ExcludeParameters = x.ExcludeParameters.Split(',').ToList().Select(y => y.Trim()).Where(y => !string.IsNullOrWhiteSpace(y)).ToList(),
                    IgnoreBody = x.IgnoreBody,
                });
        }

        public StatusCode IsRequestAuthorized(string rawUrl, NameValueCollection queryString, Stream content)
        {
            var matchingPath = PathInfos.FirstOrDefault(x => x.Regex.IsMatch(rawUrl));
            if (matchingPath != null && matchingPath.Type == PathInfo.PathType.Exclude)
            {
                return StatusCode.Authorized;
            }

            var mandatoryParameterNames = new[] { "appid", "serviceid", "userid", "expiry", _signatureParameterKey };
            if (IsMandatoryParameterMissingOrEmpty(mandatoryParameterNames, queryString))
            {
                return StatusCode.ParameterMissing;
            }

            var guidParsingSucceeded = true;
            Guid appId;
            guidParsingSucceeded &= Guid.TryParse(queryString.Get("appid"), out appId);
            Guid userId;
            guidParsingSucceeded &= Guid.TryParse(queryString.Get("userid"), out userId);
            Guid serviceId;
            guidParsingSucceeded &= Guid.TryParse(queryString.Get("serviceid"), out serviceId);
            if (!guidParsingSucceeded)
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

            var filteredQueryString = queryString;
            if (matchingPath != null && matchingPath.ExcludeParameters.Any())
            {
                filteredQueryString = new NameValueCollection();
                queryString.AllKeys.ToList().Except(matchingPath.ExcludeParameters, StringComparer.OrdinalIgnoreCase).ToList().ForEach(x => filteredQueryString.Add(x, queryString.Get(x)));
            }
            var actual = matchingPath.IgnoreBody
                ? HmacService.CalculateHash(HmacService.CreateSortedQueryString(filteredQueryString), app.Secret)
                : HmacService.CalculateHash(HmacService.CreateSortedQueryString(filteredQueryString), content, app.Secret);

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
                requiredKeys.Select(queryString.Get).Any(string.IsNullOrWhiteSpace);
        }
    }
}