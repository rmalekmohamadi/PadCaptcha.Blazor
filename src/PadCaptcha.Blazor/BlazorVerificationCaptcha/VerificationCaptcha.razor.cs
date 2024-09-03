using Microsoft.AspNetCore.Components;

namespace BlazorVerificationCaptcha
{
    public partial class VerificationCaptcha : ComponentBase
    {
        public static string ImageURL { get; private set; } = string.Empty;

        /// <summary>
        /// Generates CAPTCHA content, including an image and corresponding text, with customizable options.
        /// </summary>
        /// <param name="IncludeNumbers">Indicates whether to include numbers in the CAPTCHA text.</param>
        /// <param name="TypefaceFamilyName">The font family name for the CAPTCHA text.</param>
        /// <param name="CaptchaLength">The desired length of the CAPTCHA text.</param>
        /// <param name="JpegQualityLevel">The JPEG quality level for the image encoding.</param>
        /// <param name="ReduceRandomCharacters">Indicates whether to reduce randomness for certain characters in the image.</param>
        /// <returns>The CAPTCHA text</returns>
        public static string GenerateCaptchaContent(bool IncludeNumbers = true, string TypefaceFamilyName = "Arial", int CaptchaLength = 6, int JpegQualityLevel = 100, bool ReduceRandomCharacters = false)
        {
            (string image, string text) = CaptchaGenerator.GenerateCaptcha(IncludeNumbers, TypefaceFamilyName, CaptchaLength, JpegQualityLevel, ReduceRandomCharacters);
            Console.WriteLine("image {0}", image);
            Console.WriteLine("text {0}", text);
            Console.WriteLine("CaptchaLength {0}", CaptchaLength);
            ImageURL = image;

            return text.Replace(" ", string.Empty);
        }
    }
}