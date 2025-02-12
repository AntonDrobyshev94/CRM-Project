namespace FinalProject_Web.Interfaces
{
    public interface IImageData
    {
        Task<MultipartFormDataContent> ConvertFileToMultipartForm(IFormFile img, string imageName);
        Task<byte[]> ImageToByteArrayMethod(IFormFile image);
    }
}
