using System;
using Dragon.Security.Hmac.Module.Repositories;

namespace Dragon.Security.Hmac.Module.Services.Validators
{
    public class AppValidator : ValidatorBase
    {
        public IAppRepository AppRepository { get; set; }
        public IValidator ServiceValidator { get; set; }

        private Guid _value;

        protected override bool ParseImpl(string value)
        {
            return Guid.TryParse(value, out _value);
        }

        protected override bool ValidateImpl()
        {
            if (ServiceValidator == null)
            {
                throw new DependencyMissingException("ServiceValidator is not set.");
            }
            var serviceId = (Guid)ServiceValidator.GetValue();
            var app = AppRepository.Get(_value, serviceId);
            return (!(app == null || !app.Enabled));
        }

        protected override object GetValueImpl()
        {
            return _value;
        }
    }
}
