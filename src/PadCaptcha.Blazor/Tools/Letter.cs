using SkiaSharp;

namespace PadCaptcha.Blazor
{
    internal class Letter
    {
        public int Angle { get; set; }
        public string Value { get; set; }
        public SKColor Color { get; set; }
        public SKColor BackgroundColor { get; set; }
        public string FontFamily { get; set; }
    }
}
