using System.Net;
using FinalProject_Web.Services.Interfaces;
using FinalProject_Web.Enums;
using FinalProject_Web.Interfaces;

namespace FinalProject_Web.Data
{
    public class ImageDataService : IImageData
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authHelpServices;
        public ImageDataService(HttpClient httpClient,
            IAuthService authenticationHelpService)
        {
            _httpClient = httpClient;
            _authHelpServices = authenticationHelpService;
        }

        /// <summary>
        /// Запрос на отправку изображения, передающийся на API 
        /// сервер. Данный запрос принимает массив байтов и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="httpContext"></param>
        public async Task<HttpStatusCode> SendImageToApi(HttpContext httpContext, MultipartFormDataContent content)
        {
            string url = @"https://localhost:7037/api/image";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: content
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
            return r.StatusCode;
        }

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
            else
            {
                form = null;
                return form;
            }
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

        public async Task<UploadImageResult> UploadImageAsync(IFormFile imageFile, string imageName, HttpContext context)
        {
            var form = await ConvertFileToMultipartForm(imageFile, imageName);
            if (form != null)
            {
                if ((await SendImageToApi(context, form)).Equals(HttpStatusCode.OK))
                {
                    return UploadImageResult.Ok;
                }
                else
                {
                    return UploadImageResult.Unauthorized;
                }
            }
            else { return UploadImageResult.BadRequest; }
        }
    }
}
