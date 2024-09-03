using System.Reflection;

namespace PadCaptcha.Blazor
{
    public static class FontHelper
    {
        public static Stream? LoadStreamFont(string fontName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"PadCaptcha.Blazor.wwwroot.{fontName}";
            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}
