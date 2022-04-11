using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace APIGateway.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public List<UserRole> Roles { get; set; }

        public User() { }

        public User(string username, string password)
        {
            Username = username;
            SetPassword(password);
        }

        public void SetPassword(string password)
        {
            var salt = GenerateSalt();
            var hash = GetPasswordHash(password, salt);

            PasswordHash = Convert.ToBase64String(hash);
            Salt = Convert.ToBase64String(salt);
        }

        public bool ValidatePassword(string password)
        {
            var hash = Convert.ToBase64String(GetPasswordHash(password, Convert.FromBase64String(Salt)));
            return hash == PasswordHash;
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            return salt;
        }

        private byte[] GetPasswordHash(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000,
                numBytesRequested: 256 / 8);
        }
    }
}
