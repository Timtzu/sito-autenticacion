namespace sito_autenticacion.Model
{
    public class Usuario
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? HashedPassword { get; set; }
       
    }
}
