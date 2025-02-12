namespace FinalAuthService.AuthModels
{
    public class TokenResponseModel
    {
        public string Access_token { get; set; }
        public string Username { get; set; }
        public string RequestId { get; set; }
    }
}
