using System;

namespace Dragon.Security.Hmac.Module.Services.Validators
{
    public class ExpiryValidator : ValidatorBase
    {
        private long _value;

        protected override bool ParseImpl(string value)
        {
            return long.TryParse(value, out _value);
        }

        protected override bool ValidateImpl()
        {
            return _value >= DateTime.Now.Ticks;
        }

        protected override object GetValueImpl()
        {
            return _value;
        }
    }
}
