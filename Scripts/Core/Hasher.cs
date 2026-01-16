using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace Packkit.Core;

public static class Hasher
{
    public static string Hash(Stream stream)
    {
        using var sha = SHA256.Create();
        byte[] hashBytes = sha.ComputeHash(stream);
        string hash = hashBytes.ToString();
        return hash;
    }
}
