
namespace Aksl.Sockets.Client
{
    public static class BoolExtensions
    {
        public static bool SafeBool(this string value)
        {
            return bool.TryParse(value, out bool result) ? result : false;
        }
    }
}
