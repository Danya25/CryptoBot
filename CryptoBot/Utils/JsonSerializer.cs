using Newtonsoft.Json;

namespace CryptoBot.Utils
{
    public static class JsonSerializer
    {
        private static readonly JsonSerializerSettings Serializer = new JsonSerializerSettings();

        public static JsonSerializerSettings GetSerializerSettings() => Serializer;
    }
}
