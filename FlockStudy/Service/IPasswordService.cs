using System.Security.Cryptography;
using System.Text;

namespace FlockStudy.Service
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            // Hash password with salt
            using var sha256 = SHA256.Create();
            var saltedPassword = password + salt;
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            var hash = Convert.ToBase64String(hashedBytes);

            // Return salt and hash combined
            return $"{salt}:{hash}";
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                var parts = hash.Split(':');
                if (parts.Length != 2) return false;

                var salt = parts[0];
                var storedHash = parts[1];

                // Hash the provided password with the stored salt
                using var sha256 = SHA256.Create();
                var saltedPassword = password + salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                var computedHash = Convert.ToBase64String(hashedBytes);

                // Compare hashes
                return storedHash == computedHash;
            }
            catch
            {
                return false;
            }
        }
    }
}
