using System.ComponentModel.DataAnnotations;

namespace Dragon.SecurityServer.Demo.Models
{
    public class UpdateProfileClaimsViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Address")]
        public string Address { get; set; }
    }
}