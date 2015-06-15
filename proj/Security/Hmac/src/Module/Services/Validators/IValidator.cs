namespace Dragon.Security.Hmac.Module.Services.Validators
{
    public interface IValidator
    {
        bool Parse(string value);
        bool Validate();
        object GetValue();
        bool IsOptional();
        void OnSuccess();
    }
}
