using System.ComponentModel.DataAnnotations;

namespace Auth_API.Models.Dtos
{
    public class UserAuthLogin
    {
        [Required(ErrorMessage = "El correo es necesario")]
        public string UserEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
