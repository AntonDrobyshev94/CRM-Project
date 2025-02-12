using FinalProject_Web.Interfaces;
using ServicesLibrary.Enums;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicService.Interfaces;

namespace FinalProject_Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly IImageData _imageData;
        private readonly IImageDataService _imageDataService;
        public ImageController(IImageData imageData,
            IImageDataService imageDataService)
        {
            _imageData = imageData;
            _imageDataService = imageDataService;
        }
       
        /// <summary>
        /// POST метод загрузки и передачи изображения в API
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="imageName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadAndSendImage(IFormFile imageFile, string imageName)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            var multipartFormDataModel = await _imageData.ConvertFileToMultipartForm(imageFile, imageName);
            var result = await _imageDataService.UploadImageAsync(multipartFormDataModel, token, "https://localhost:7037/api/image");
            switch (result)
            {
                case UploadImageResult.Ok:
                    return RedirectToAction("Index");
                case UploadImageResult.Unauthorized:
                    return Unauthorized();
                case UploadImageResult.BadRequest:
                default:
                    return BadRequest();
            }
        }
    }
}
