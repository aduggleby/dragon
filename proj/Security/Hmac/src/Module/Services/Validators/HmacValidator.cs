using System;
using System.Collections.Specialized;
using Dragon.Security.Hmac.Core.Service;
using Dragon.Security.Hmac.Module.Repositories;

namespace Dragon.Security.Hmac.Module.Services.Validators
{
    public class HmacValidator : ValidatorBase
    {
        public IHmacService HmacService { get; set; }
        public IAppRepository AppRepository { get; set; }
        public IValidator AppValidator { get; set; }
        public IValidator ServiceValidator { get; set; }

        private NameValueCollection _queryString;

        private string _value;

        public void SetQueryString(NameValueCollection queryString)
        {
            _queryString = queryString;
        }

        protected override bool ParseImpl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            _value = value;
            return true;
        }

        protected override bool ValidateImpl()
        {
            if (AppValidator == null || ServiceValidator == null)
            {
                throw new DependencyMissingException("AppValidator and/or ServiceValidator are not set.");
            }
            var appId = (Guid)AppValidator.GetValue();
            var serviceId = (Guid)ServiceValidator.GetValue();
            var app = AppRepository.Get(appId, serviceId);
            var actual = HmacService.CalculateHash(HmacService.CreateSortedQueryString(_queryString), app.Secret);
            return (actual.ToLower() == _value.ToLower());
        }

        protected override object GetValueImpl()
        {
            return _value;
        }
    }
}
