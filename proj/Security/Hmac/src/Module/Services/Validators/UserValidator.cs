using System;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;

namespace Dragon.Security.Hmac.Module.Services.Validators
{
    public class UserValidator : ValidatorBase
    {
        public IUserRepository UserRepository { get; set; }
        public IValidator ServiceValidator { get; set; }
        public IValidator AppValidator { get; set; }

        private Guid _value;
        private UserModel _user;
        private Guid _serviceId;

        public override void OnSuccess()
        {
            if (_user != null) return;

            var appId = (Guid)AppValidator.GetValue();
            UserRepository.Insert(new UserModel
            {
                AppId = appId,
                ServiceId = _serviceId,
                Enabled = true,
                UserId = _value,
                CreatedAt = DateTime.Now
            });
        }

        protected override bool ParseImpl(string value)
        {
            return Guid.TryParse(value, out _value);
        }

        protected override bool ValidateImpl()
        {
            if (ServiceValidator == null || AppValidator == null)
            {
                throw new DependencyMissingException("AppValidator and/or ServiceValidator are not set.");
            }
            _serviceId = (Guid)ServiceValidator.GetValue();
            _user = UserRepository.Get(_value, _serviceId);
            return (_user == null || _user.Enabled);
        }

        protected override object GetValueImpl()
        {
            return _value;
        }
    }
}
