using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace APIGateway.Models
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordConfirm { get; set; }

        public bool IsValid()
        {
            return UsernameIsValid() && PasswordIsValid();
        }

        public bool UsernameIsValid()
        {
            return !string.IsNullOrEmpty(Username) && Regex.IsMatch(Username, @"^[a-zA-Z0-9_]+$");
        }

        public bool PasswordIsValid()
        {
            return Password != null && Password == PasswordConfirm ;
        }
    }
}
