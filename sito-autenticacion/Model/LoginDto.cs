using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sito_autenticacion.Model
{
    public class LoginDto
    {
        [Required]
        [StringLength(50)]
        public string? Username { get; set; }

        [PasswordPropertyText]
        [Required]
        [StringLength(50)]
        public string? Password { get; set; }
    }
}
