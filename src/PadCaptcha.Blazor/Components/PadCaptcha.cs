using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using SkiaSharp;
using Microsoft.AspNetCore.Components.Rendering;
using System.Reflection;
using System.Drawing.Text;

namespace PadCaptcha.Blazor
{
    public class PadCaptcha : ComponentBase
    {
        [Parameter] public string Chars { get; set; } = "ABCDEFGHJKLMNPQRSTUWYZabcdefghijkmnpqrstuwz23456789";
        [Parameter] public int Width { get; set; } = 170;
        [Parameter] public int Height { get; set; } = 40;
        [Parameter] public int CharLength { get; set; } = 5;
        [Parameter] public bool CaseSensitive { get; set; } = false;
        [Parameter] public bool RefreshOnFailed { get; set; } = false;
        [Parameter] public int? LimitFaildPerMinute { get; set; }
        [Parameter] public int? LimitRefreshPerMinute { get; set; }
        [Parameter] public string TextColor { get; set; }
        [Parameter] public string BackgroundColor { get; set; }
        [Parameter] public bool Lines { get; set; } = true;
        [Parameter] public bool Dots { get; set; } = true;
        [Parameter] public string CaptchaWrapperClasses { get; set; }
        [Parameter] public string RefreshBtnClasses { get; set; }
        [Parameter] public string CaptchaClasses { get; set; }
        [Parameter] public string CaptchaWrapperStyle { get; set; } = "display:flex";
        [Parameter] public string RefreshBtnStyle { get; set; } = "width:40px;height:40px;";
        [Parameter] public string CaptchaStyle { get; set; }


        //[Parameter] public EventCallback<string> CaptchaWordChanged { get; set; }
        [Parameter] public RenderFragment RefreshBtnTemplate { get; set; }

        private string _captchaWord;
        private int _failedCounter = 0;
        private int _refreshCounter = 0;
        private string _img = "";

        protected override async Task OnInitializedAsync()
        {
            Rebuild();
            //if (!string.IsNullOrEmpty(_captchaWord))
            //{
            //    await CaptchaWordChanged.InvokeAsync(_captchaWord);
            //}
            await base.OnInitializedAsync();
        }

        private void Rebuild()
        {
            var captchaWord = CaptchaWordTools.Generate(Chars, CharLength);
            _captchaWord = CaptchaWordTools.Hash(CaseSensitive ? captchaWord : captchaWord.ToLower());

            var randomValue = new Random();

            var bgColor = GetColor(BackgroundColor, (byte)randomValue.Next(70, 100), (byte)randomValue.Next(60, 80), (byte)randomValue.Next(50, 90));

            var fontFamilies = new string[] { "ARIAL.TTF", "COUR.TTF", "TIMES.TTF", "VERDANA.TTF" };

            var letters = new List<Letter>();

            if (!string.IsNullOrEmpty(captchaWord))
            {
                foreach (char c in captchaWord)
                {
                    var color = GetColor(TextColor, (byte)randomValue.Next(100, 256), (byte)randomValue.Next(110, 256), (byte)randomValue.Next(90, 256));
                    var letter = new Letter
                    {
                        Value = c.ToString(),
                        Angle = randomValue.Next(-15, 25),
                        Color = color,
                        FontFamily = fontFamilies[randomValue.Next(0, fontFamilies.Length)],
                    };

                    letters.Add(letter);
                }

                SKImageInfo imageInfo = new(Width, Height);
                using (var surface = SKSurface.Create(imageInfo))
                {
                    var canvas = surface.Canvas;
                    canvas.Clear(bgColor);

                    using (SKPaint paint = new())
                    {
                        float x = 10;

                        foreach (Letter l in letters)
                        {
                            using (var typeface = SKTypeface.FromStream(FontHelper.LoadStreamFont(l.FontFamily)))
                            {
                                paint.Typeface = typeface;
                            }

                            paint.Color = l.Color;
                            paint.TextAlign = SKTextAlign.Left;
                            paint.TextSize = randomValue.Next(Height / 2, (Height / 2) + (Height / 4));
                            paint.FakeBoldText = true;
                            paint.IsAntialias = true;

                            SKRect rect = new();
                            float width = paint.MeasureText(l.Value, ref rect);

                            float textWidth = width;
                            var y = (Height - rect.Height);

                            canvas.Save();

                            canvas.RotateDegrees(l.Angle, x, y);
                            canvas.DrawText(l.Value, x, y, paint);

                            canvas.Restore();

                            x += textWidth + 10;

                            if (Dots)
                            {
                                for (int i = 0; i <= 4; i++)
                                {
                                    paint.Color = new SKColor((byte)randomValue.Next(100, 256), (byte)randomValue.Next(110, 256), (byte)randomValue.Next(90, 256), (byte)randomValue.Next(40, 100));
                                    canvas.DrawCircle(randomValue.Next(0, Width), randomValue.Next(0, Height), randomValue.Next(0, 4), paint);
                                }
                            }
                        }

                        if (Lines)
                        {
                            canvas.DrawLine(0, randomValue.Next(0, Height), Width, randomValue.Next(0, Height), paint);
                            canvas.DrawLine(0, randomValue.Next(0, Height), Width, randomValue.Next(0, Height), paint);
                        }

                        paint.Style = SKPaintStyle.Stroke;
                        canvas.DrawOval(randomValue.Next(-Width, Width), randomValue.Next(-Height, Height), Width, Height, paint);
                    }


                    // save the file
                    MemoryStream memoryStream = new();
                    using (var image = surface.Snapshot())
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 75))
                        data.SaveTo(memoryStream);
                    string imageBase64Data2 = Convert.ToBase64String(memoryStream.ToArray());
                    _img = string.Format("data:image/gif;base64,{0}", imageBase64Data2);
                }
            }

        }

        private void Refresh()
        {
            Rebuild();
            //await CaptchaWordChanged.InvokeAsync(_captchaWord);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var seq = 0;
            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "style", CaptchaWrapperStyle);
            builder.AddAttribute(++seq, "class", "captcha-wrapper " + CaptchaWrapperClasses); 
            {
                builder.OpenElement(++seq, "img");
                builder.AddAttribute(++seq, "src", _img);
                builder.AddAttribute(++seq, "class", CaptchaClasses);
                builder.AddAttribute(++seq, "style", CaptchaStyle);
                builder.CloseElement();
                builder.OpenElement(++seq, "button");
                {
                    builder.AddAttribute(++seq, "class", "captcha-refresh-btn " + RefreshBtnClasses);
                    builder.AddAttribute(++seq, "style", RefreshBtnStyle);
                    builder.AddAttribute(++seq, "type", "button");
                    builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => Refresh()));
                    builder.OpenElement(++seq, "svg");
                    {
                        builder.AddAttribute(++seq, "aria-hidden", true);
                        builder.AddAttribute(++seq, "xmlns", "http://www.w3.org/2000/svg");
                        builder.AddAttribute(++seq, "width", "24");
                        builder.AddAttribute(++seq, "height", "24");
                        builder.AddAttribute(++seq, "fill", "none");
                        builder.AddAttribute(++seq, "viewBox", "0 0 24 24");
                        builder.OpenElement(++seq, "path");
                        {
                            builder.AddAttribute(++seq, "stroke", "currentColor");
                            builder.AddAttribute(++seq, "stroke-linecap", "round");
                            builder.AddAttribute(++seq, "stroke-linejoin", "round");
                            builder.AddAttribute(++seq, "stroke-width", "2");
                            builder.AddAttribute(++seq, "d", "M17.651 7.65a7.131 7.131 0 0 0-12.68 3.15M18.001 4v4h-4m-7.652 8.35a7.13 7.13 0 0 0 12.68-3.15M6 20v-4h4");
                        }
                        builder.CloseElement();
                    }
                    builder.CloseElement();
                }
                builder.CloseElement();
            }
            builder.CloseElement();


            base.BuildRenderTree(builder);
        }

        public bool IsValid(string text)
        {
            var result = CaptchaWordTools.Verif(CaseSensitive ? text : text?.ToLower() ?? "", _captchaWord);
            if(!result && RefreshOnFailed)
            {
                Refresh();
                StateHasChanged();
            }
            return result;
        }

        private SKColor GetColor(string hexColor, byte red, byte green, byte blue)
        {
            var color = new SKColor(red, green, blue);

            if (!string.IsNullOrEmpty(hexColor))
            {
                var result = SKColor.TryParse(hexColor, out SKColor colorTmp);
                if (result)
                {
                    color = colorTmp;
                }
            }

            return color;
        }

    }
}
