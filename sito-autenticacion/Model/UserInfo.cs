namespace sito_autenticacion.Model
{
   
    public class ClaimDto
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class UserInfo
    {
        public bool IsAuthenticated { get; set; }
        public string? Name { get; set; }
        public List<ClaimDto> Claims { get; set; } = new();
    }
}
