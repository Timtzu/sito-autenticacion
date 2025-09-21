using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace sito_autenticacion.Model
{
    public class RegistroDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [AllowNull]
        public string Role { get; set; } = "Alumno";
    }
}
