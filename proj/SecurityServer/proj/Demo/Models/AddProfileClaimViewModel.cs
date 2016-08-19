using System.ComponentModel.DataAnnotations;

namespace Dragon.SecurityServer.Demo.Models
{
    public class AddProfileClaimViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Value")]
        public string Value { get; set; }
    }
}