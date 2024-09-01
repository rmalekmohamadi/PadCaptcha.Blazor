using System.Text;
using System.Security.Cryptography;

namespace PadCaptcha.Blazor
{
    internal static class CaptchaWordTools
    {
        public static string Generate(string chars, int length)
        {
            var random = new Random(DateTime.Now.Millisecond);

            string cw = new(Enumerable.Repeat(chars, length)
                                      .Select(s => s[random.Next(s.Length)])
                                      .ToArray());

            return cw;
        }

        public static string Hash(string source)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return GetHash(sha256Hash, source);
            }
        }

        public static bool Verif(string source, string hash)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return VerifyHash(sha256Hash, source, hash);
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            if(string.IsNullOrEmpty(input)) 
            {
                input = "";
            }

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            // Hash the input.
            var hashOfInput = GetHash(hashAlgorithm, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}
