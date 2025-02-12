using FinalProject_Web.Interfaces;

namespace FinalProject_Web.Data
{
    public class ImageData : IImageData
    {
        public ImageData(HttpClient httpClient)
        { }
        /// <summary>
        /// Метод конвертации изображения в MultipartFormDataContentё для передачи
        /// в теле POST запроса 
        /// </summary>
        /// <param name="img"></param>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public async Task<MultipartFormDataContent> ConvertFileToMultipartForm(IFormFile img, string imageName)
        {
            var form = new MultipartFormDataContent();
            byte[] fileBytes;
            if (img != null)
            {
                fileBytes = await ImageToByteArrayMethod(img);
                form.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "imageFile", imageName);
                return form;
            }
            else return null;
        }

        /// <summary>
        /// Метод чтения изображения и конвертации в массив байтов
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<byte[]> ImageToByteArrayMethod(IFormFile image)
        {
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
                return fileBytes;
            }
        }
    }
}
