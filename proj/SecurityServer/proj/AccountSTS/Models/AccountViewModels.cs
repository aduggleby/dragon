using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Owin.Security;

namespace Dragon.SecurityServer.AccountSTS.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [Display(Name = "Email", ResourceType = typeof(Resources.Global))]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Global))]
        public string Password { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
        public List<AuthenticationDescription> AvailableProviders { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        public string Provider { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [Display(Name = "Code", ResourceType = typeof(Resources.Global))]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "RememberBrowser", ResourceType = typeof(Resources.Global))]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [Display(Name = "Email", ResourceType = typeof(Resources.Global))]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [Display(Name = "Email", ResourceType = typeof(Resources.Global))]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "EmailAddressAttribute_ValidationError")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Global))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Resources.Global))]
        public bool RememberMe { get; set; }

        public List<AuthenticationDescription> AvailableProviders { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "EmailAddressAttribute_ValidationError")]
        [Display(Name = "Email", ResourceType = typeof(Resources.Global))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "StringLengthAttribute_ValidationError", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Global))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.Global))]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "CompareAttribute_ValidationError")]
        public string ConfirmPassword { get; set; }
    }

    public class UpdateViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [DataType(DataType.Text)]
        [Display(Name = "Id", ResourceType = typeof(Resources.Global))]
        public string Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "EmailAddressAttribute_ValidationError")]
        [Display(Name = "Email", ResourceType = typeof(Resources.Global))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "StringLengthAttribute_ValidationError", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Global))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.Global))]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "CompareAttribute_ValidationError")]
        public string ConfirmPassword { get; set; }
    }
    
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "EmailAddressAttribute_ValidationError")]
        [Display(Name = "Email", ResourceType = typeof(Resources.Global))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "StringLengthAttribute_ValidationError", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Global))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.Global))]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "CompareAttribute_ValidationError")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredAttribute_ValidationError")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "EmailAddressAttribute_ValidationError")]
        [Display(Name = "Email", ResourceType = typeof(Resources.Global))]
        public string Email { get; set; }
    }
}
