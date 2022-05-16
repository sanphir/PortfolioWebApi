using System.Security.Cryptography;

namespace StudyProj.WebApp.Security
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        //TO DO 
        //hash password using PBKDF2 with SHA-256, with 128 bit salt and 10000 iterations
        //KeyDerivation.Pbkdf2 is same as Rfc2898DeriveBytes

        //TO DO
        //test on same passwords

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
                throw new FormatException($"Unexpected hash format.");
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
