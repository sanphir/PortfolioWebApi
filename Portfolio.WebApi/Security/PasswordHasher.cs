using System.Security.Cryptography;

namespace Portfolio.WebApi.Security
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const int SALT_SIZE = 16; // 128 bit 
        private const int KEY_SIZE = 32; // 256 bit

        public string Hash(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(password, SALT_SIZE))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KEY_SIZE));
                var salt = Convert.ToBase64String(algorithm.Salt);

                return $"{salt}.{key}";
            }
        }

        public bool IsPasswordValid(string hash, string password)
        {
            var parts = hash.Split('.', 2);

            if (parts.Length != 2)
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[0]);
            var key = Convert.FromBase64String(parts[1]);

            using (var algorithm = new Rfc2898DeriveBytes(password, salt))
            {
                var keyToCheck = algorithm.GetBytes(KEY_SIZE);
                return keyToCheck.SequenceEqual(key);
            }
        }
    }
}
