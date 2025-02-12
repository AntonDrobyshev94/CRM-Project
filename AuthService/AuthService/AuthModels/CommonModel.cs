using System.Text.Json;

namespace AuthService.AuthModels
{
    public class CommonModel
    {
        public string Type { get; set; }
        public JsonElement BaseModel { get; set; }
    }
}
