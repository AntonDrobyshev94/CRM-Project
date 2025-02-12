using System.Net;
using BusinessLogicService.Interfaces;
using ServicesLibrary.Enums;
using ServicesLibrary.Services.Interfaces;

namespace BusinessLogicService.Data
{
    public class ImageDataService : BaseDataService, IImageDataService
    {
        public ImageDataService(HttpClient httpClient,
            IAuthService authService) : base(httpClient, authService)
        { }

        /// <summary>
        /// Запрос на отправку изображения, передающийся на API 
        /// сервер. Данный запрос принимает массив байтов и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task<HttpStatusCode> SendImageToApi(string token, string url, MultipartFormDataContent content)
        {
            var r = await PostAsync(url, content, token);
            return r.StatusCode;
        }

        /// <summary>
        /// Метод загрузки изображения
        /// </summary>
        /// <param name="form"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<UploadImageResult> UploadImageAsync(MultipartFormDataContent form, string token, string url)
        {
            if (form != null)
            {
                var response = await SendImageToApi(token, url, form);
                if (response.Equals(HttpStatusCode.OK))
                {
                    return UploadImageResult.Ok;
                }
                else if (response.Equals(HttpStatusCode.Unauthorized))
                {
                    return UploadImageResult.Unauthorized;
                }
                else return UploadImageResult.BadRequest;
                
            }
            else return UploadImageResult.BadRequest;
        }
    }
}
