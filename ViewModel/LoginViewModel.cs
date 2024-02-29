using System.ComponentModel.DataAnnotations;

namespace Arkenea_new.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name ="Email Address")]
        [Required(ErrorMessage = "Email Address Required")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
