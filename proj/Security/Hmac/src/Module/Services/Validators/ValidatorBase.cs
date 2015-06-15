namespace Dragon.Security.Hmac.Module.Services.Validators
{
    public abstract class ValidatorBase : IValidator
    {
        private bool _isOptional = false;
        private bool _isParsed = false;

        public bool Parse(string value)
        {
            var success = ParseImpl(value);
            if (success)
            {
                _isParsed = true;
            }
            return success;
        }

        public bool Validate()
        {
            if (!IsParsed())
            {
                throw new NotYetParsedException("Call Parse() before validating.");
            }
            return ValidateImpl();
        }

        public object GetValue()
        {
            if (!IsParsed())
            {
                throw new NotYetParsedException("Call Parse() before getting the value.");
            }
            return GetValueImpl();
        }

        public bool IsOptional()
        {
            return _isOptional;
        }

        public virtual void OnSuccess()
        {
            // nothing to be done
        }

        protected abstract bool ParseImpl(string value);
        protected abstract bool ValidateImpl();
        protected abstract object GetValueImpl();

        private bool IsParsed()
        {
            return _isParsed;
        }
    }
}
