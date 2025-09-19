namespace sito_autenticacion.Model
{
    public class RegistroDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; } = "Alumno";
    }
}
