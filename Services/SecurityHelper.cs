using System.Security.Cryptography;
using System.Text;

namespace Cotalog.Services
{
    public static class SecurityHelper
    {
        public static string GenerateSalt()
        {
            var bytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPassword(string password, string salt)
        {
            var saltedPassword = password + salt;
            return Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(saltedPassword)));
        }
    }
}