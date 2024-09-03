using SkiaSharp;

namespace BlazorVerificationCaptcha
{
    public static class CaptchaGenerator
    {
        private static readonly int minCaptchaLength = 6;
        private static readonly int baseWidth = 220;
        private static readonly int baseHeight = 50;

        /// <summary>
        /// Generates a CAPTCHA image with customizable options.
        /// </summary>
        /// <param name="IncludeNumbers">Include numbers in the CAPTCHA text.</param>
        /// <param name="TypefaceFamilyName">The font family name for the CAPTCHA text.</param>
        /// <param name="CaptchaLength">The length of the CAPTCHA text.</param>
        /// <param name="JpegQualityLevel">The JPEG quality level for the image.</param>
        /// <param name="ReduceRandomCharacters">Reduce the random characters a bit.</param>
        /// <returns>A tuple containing the image encoded as base64 and the CAPTCHA text.</returns>
        public static (string imagevalue, string textvalue) GenerateCaptcha(bool IncludeNumbers, string TypefaceFamilyName, int CaptchaLength, int JpegQualityLevel, bool ReduceRandomCharacters)
        {
            int effectiveCaptchaLength = Math.Max(CaptchaLength, minCaptchaLength);
            int dynamicWidth = baseWidth + (effectiveCaptchaLength - minCaptchaLength) * 10;

            int dynamicHeight = baseHeight;
            string captchaText = Tools.GenerateRandomText(IncludeNumbers, CaptchaLength);

            using SKBitmap bitmap = new(dynamicWidth, dynamicHeight);
            using SKCanvas canvas = new(bitmap);

            using SKPaint paint = new();
            canvas.Clear(RandomColor(false));

            paint.TextSize = Tools.RandomFloatValue(19, 25);
            paint.Color = SKColors.Black;

            using (SKTypeface typeface = SKTypeface.FromFamilyName(TypefaceFamilyName))
            using (SKFont font = new(typeface, paint.TextSize))
            {
                paint.ImageFilter = SKImageFilter.CreateBlur(1, 1);
                paint.FilterQuality = SKFilterQuality.Low;

                paint.TextSkewX = 1;
                float textWidth = paint.MeasureText(captchaText);

                float textXCord = (dynamicWidth - textWidth) / 2;
                float textYCord = Tools.RandomFloatValue(26, 35);

                canvas.DrawText(captchaText, textXCord, textYCord, paint);
                RandomNumbersAndText(canvas, paint, CaptchaLength, ReduceRandomCharacters);
            }

            using SKImage image = SKImage.FromBitmap(bitmap);
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, JpegQualityLevel);

            return (ConvStreamToBase64(data), captchaText);
        }

        /// <summary>
        /// Adds random characters to the canvas to create additional complexity in the CAPTCHA image.
        /// </summary>
        /// <param name="Canvas">The SKCanvas object to draw on.</param>
        /// <param name="Paint">The SKPaint object to define text rendering properties.</param>
        /// <param name="CaptchaLength">The total length of the CAPTCHA text.</param>
        /// <param name="ReduceRandomCharacters">Flag indicating whether to reduce random characters.</param>
        private static void RandomNumbersAndText(SKCanvas Canvas, SKPaint Paint, int CaptchaLength, bool ReduceRandomCharacters)
        {
            for (int i = 0; i < CaptchaLength; i++)
            {
                char randomChar;
                if (ReduceRandomCharacters && i % 2 == 0)
                {
                    randomChar = ' ';
                }
                else
                {
                    randomChar = Tools.AlphabetAndNumbers[Convert.ToInt32(Tools.RandomFloatValue(0, Tools.AlphabetAndNumbers.Length))];
                }

                string randomString = randomChar.ToString();
                Paint.TextSize = Tools.RandomFloatValue(12, 18);

                Paint.Color = RandomColor(true);
                float xCord = Tools.RandomFloatValue(0, Convert.ToInt32(Canvas.LocalClipBounds.Width));

                float yCord = Tools.RandomFloatValue(0, Convert.ToInt32(Canvas.LocalClipBounds.Height));
                Canvas.DrawText(randomString, xCord, yCord, Paint);
            }
        }

        /// <summary>
        /// Converts the provided SKData to a base64-encoded string with a data URI for image embedding.
        /// </summary>
        /// <param name="Data">The SKData to be converted.</param>
        /// <returns>A base64-encoded string representing the provided image data.</returns>
        private static string ConvStreamToBase64(SKData Data)
        {
            MemoryStream ms = new();
            Data.SaveTo(ms);

            byte[] byteArray = ms.ToArray();
            string b64String = Convert.ToBase64String(byteArray);

            return $"data:image/jpg;base64,{b64String}";
        }

        /// <summary>
        /// Generates a random color for the background or foreground of the CAPTCHA image.
        /// </summary>
        /// <param name="IsForegroundColorNeeded">Flag indicating whether a foreground color is needed.</param>
        /// <returns>A random SKColor object representing the selected color.</returns>
        private static SKColor RandomColor(bool IsForegroundColorNeeded = true)
        {
            if (IsForegroundColorNeeded)
            {
                return Tools.ForegroundColors[Random.Shared.Next(Tools.ForegroundColors.Length)];
            }

            return Tools.BackgroundColors[Random.Shared.Next(Tools.BackgroundColors.Length)];
        }
    }
}