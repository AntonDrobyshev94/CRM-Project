using System.Text.Json;

namespace FinalProject_Web.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}
