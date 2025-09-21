using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace sito_autenticacion.Model
{
    public class Usuario
    {
        [AllowNull]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Username { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        [MaxLength(50)]
        public string? Role { get; set; }



    }
}
