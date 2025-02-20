using System.Security.Cryptography;
using System.Text;

namespace TestPayTech.Helpers;

public static class Utils
{
    public static bool VerifyPaytechRequest(string apiKeySha256, string apiSecretSha256, string myApiKey, string myApiSecret)
    {
        string encryptedApiKey = Sha256Encrypt(myApiKey);
        string encryptedApiSecret = Sha256Encrypt(myApiSecret);
        return encryptedApiKey == apiKeySha256 && encryptedApiSecret == apiSecretSha256;
    }

    private static string Sha256Encrypt(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public static class StatutPaiement
    {
        public static string Done = "sale_complete";
        public static string Canceled = "sale_canceled";
    }
}