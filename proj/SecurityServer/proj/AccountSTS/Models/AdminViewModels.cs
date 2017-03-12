using System.ComponentModel.DataAnnotations;

namespace Dragon.SecurityServer.AccountSTS.Models
{
    public class FindUserViewModel
    {
        [Required]
        [MinLength(3)]
        [Display(Name = "SearchTerm")]
        public string SearchTerm { get; set; }
    }

    public class FindUserResultViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
