using System.Net;
using FinalProject_Web.Enums;

namespace FinalProject_Web.Interfaces
{
    public interface IImageData
    {
        Task<HttpStatusCode> SendImageToApi(HttpContext httpContext, MultipartFormDataContent content);
        Task<byte[]> ImageToByteArrayMethod(IFormFile image);
        Task<MultipartFormDataContent> ConvertFileToMultipartForm(IFormFile img, string imageName);
        Task<UploadImageResult> UploadImageAsync(IFormFile imageFile, string imageName, HttpContext context);
    }
}
