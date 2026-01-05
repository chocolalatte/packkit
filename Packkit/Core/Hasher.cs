using System.IO.Pipelines;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace Packkit.Core
{
    public static class Hasher
    {
        public static string Hash(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var sha = SHA256.Create();
            byte[] hashBytes = sha.ComputeHash(stream);
            return Convert.ToHexStringLower(hashBytes);
        }
    }
}