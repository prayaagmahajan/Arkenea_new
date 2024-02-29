using System.ComponentModel.DataAnnotations;

namespace Arkenea_new.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
