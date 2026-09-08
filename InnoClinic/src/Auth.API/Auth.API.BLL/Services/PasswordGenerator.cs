using InnoClinic.Auth.API.BLL.Interfaces;
using System.Security.Cryptography;

namespace InnoClinic.Auth.API.BLL.Services
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
        private const int Length = 12;

        public string Generate()
        {
            return RandomNumberGenerator.GetString(Chars, Length);
        }
    }
}
