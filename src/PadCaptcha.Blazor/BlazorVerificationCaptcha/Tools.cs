using SkiaSharp;
using System.Text;

namespace BlazorVerificationCaptcha
{
    internal static class Tools
    {
        internal static readonly string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        internal static readonly string AlphabetAndNumbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        internal static readonly SKColor[] ForegroundColors = new SKColor[] { SKColors.Black, SKColors.DarkBlue, SKColors.DarkGreen, SKColors.DarkViolet };
        internal static readonly SKColor[] BackgroundColors = new SKColor[] { SKColors.White, SKColors.Yellow, SKColors.LightBlue, SKColors.OrangeRed };
        private static readonly Random randi = new();

        private static StringBuilder RandomCaptchaText { get; set; } = new();

        /// <summary>
        /// Generates a random text containing letters and optionally numbers for CAPTCHA purposes.
        /// </summary>
        /// <param name="UseNumbers">Indicates whether to include numbers in the generated text.</param>
        /// <param name="CaptchaLength">The desired length of the generated text.</param>
        /// <param name="AddSpaces">Indicates whether to add spaces between characters for better readability.</param>
        /// <returns>A randomly generated string containing letters and optionally numbers.</returns>
        internal static string GenerateRandomText(bool UseNumbers, int CaptchaLength, bool AddSpaces = true)
        {
            RandomCaptchaText.Clear();
            for (int i = 0; i < CaptchaLength; i++)
            {
                if (UseNumbers)
                {
                    RandomCaptchaText.Append(AlphabetAndNumbers[Random.Shared.Next(AlphabetAndNumbers.Length)]);
                }
                else
                {
                    RandomCaptchaText.Append(Alphabet[Random.Shared.Next(Alphabet.Length)]);
                }

                if (AddSpaces && i < CaptchaLength - 1)
                {
                    RandomCaptchaText.Append(' ');
                }
            }

            return RandomCaptchaText.ToString();
        }

        internal static float RandomFloatValue(int Min, int Max)
        {
            return randi.Next(Min, Max);
        }
    }
}