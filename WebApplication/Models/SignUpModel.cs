using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class SignUpModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is mandatory.")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is mandatory.")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string PasswordConfirm { get; set; }
    }
}
