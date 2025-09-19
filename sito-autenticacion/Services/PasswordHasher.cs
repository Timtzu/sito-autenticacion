using System.Security.Cryptography;

namespace sito_autenticacion.Services
{
    public class PasswordHasher
    {
        private const int SaltSize = 8; // 64 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 16; // Adjustable based on needs


        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // Hash the password with PBKDF2 using SHA256
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Convert salt and hash to Base64 for storage
            string saltBase64 = Convert.ToBase64String(salt);
            string hashBase64 = Convert.ToBase64String(hash);

            // Combine iterations, salt, and hash into a single string
            return $"{Iterations}:{saltBase64}:{hashBase64}";
        }


        public bool VerifyPassword(string password, string storedHashString)
        {
            // Split the stored hash string into its components
            string[] parts = storedHashString.Split(':');
            if (parts.Length != 3)
            {
                return false; // Invalid format
            }

            // Parse iterations, salt, and hash
            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] storedHash = Convert.FromBase64String(parts[2]);

            // Hash the provided password with the stored salt and iterations
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Compare hashes in constant time to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(hash, storedHash);
        }
    }
}