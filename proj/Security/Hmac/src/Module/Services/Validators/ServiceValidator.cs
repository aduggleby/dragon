using System;

namespace Dragon.Security.Hmac.Module.Services.Validators
{
    public class ServiceValidator : ValidatorBase
    {
        private Guid _value;
        private readonly string _serviceId;

        public ServiceValidator(string serviceId)
        {
            _serviceId = serviceId;
        }

        protected override bool ParseImpl(string value)
        {
            return Guid.TryParse(value, out _value);
        }

        protected override bool ValidateImpl()
        {
            return (_value.ToString() == _serviceId);
        }

        protected override object GetValueImpl()
        {
            return _value;
        }
    }
}
