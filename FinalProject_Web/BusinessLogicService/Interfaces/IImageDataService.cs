using System.Net;
using ServicesLibrary.Enums;

namespace BusinessLogicService.Interfaces
{
    public interface IImageDataService
    {
        Task<HttpStatusCode> SendImageToApi(string token, string url, MultipartFormDataContent content);
        Task<UploadImageResult> UploadImageAsync(MultipartFormDataContent content, string token, string url);
    }
}
