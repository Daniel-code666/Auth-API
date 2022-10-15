using System.ComponentModel.DataAnnotations;

namespace Auth_API.Models.Dtos
{
    public class UserAuthDto
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Es necesario un nombre")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El correo es necesario")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "La contraseña debe se mínimo de 4 caracteres y máximo de 10")]
        public string Password { get; set; }
    }
}
