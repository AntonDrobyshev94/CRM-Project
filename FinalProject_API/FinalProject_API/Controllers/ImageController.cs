using FinalProject_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_API.Controllers
{
    /// <summary>
    /// Контроллер, осуществляющий взаимодействие с изображениями
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : Controller
    {
        private readonly ImageData _imageData;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ImageController"/> с инжекцией 
        /// зависимостей для <see cref="ImageData"/>.
        /// </summary>
        /// <param name="imageData">Экземпляр <see cref="ImageData"/>, 
        /// используемый для взаимодействия с изображениями.</param>
        public ImageController(ImageData imageData)
        {
            _imageData = imageData;
        }

        #region NODbImage
        /// <summary>
        /// POST запрос для изменения изображения
        /// </summary>
        /// <param name="imageFile">Модель файла формата IFormFile</param>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ChangeImage(IFormFile imageFile)
        {
            bool isChangeImage = await _imageData.ChangeNoDBImage(imageFile);
            if (isChangeImage)
            {
                return Ok("Файл сохранён");
            }
            else
            {
                return BadRequest("Файл изображения не найден или пуст.");
            }
        }

        /// <summary>
        /// Метод, вызывающийся GET запросом, возвращающий массив битов, 
        /// содержащий изображение
        /// </summary>
        /// <param name="fileName">Имя файла изображения</param>
        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetNoDBImage(string fileName)
        {
            var imageBytes = await _imageData.GetNoDBImage(fileName);
            if (imageBytes == null) { return NotFound(); }
            return File(imageBytes, "image/png");
        }
        #endregion

        #region DB_Image
        /// <summary>
        /// Метод, вызывающийся GET запросом, возвращающий массив битов,
        /// содержащий изображение
        /// </summary>
        /// <param name="typeOfModel">Тип модели</param>
        /// <param name="uniqueName">Уникальное имя модели</param>
        [HttpGet("{typeOfModel}/{uniqueName}")]
        public async Task<IActionResult> GetByteImage(string typeOfModel, string uniqueName)
        {
            return File(await _imageData.GetImageFromDB(typeOfModel, uniqueName), "image/png");
        }
        #endregion
    }
}
