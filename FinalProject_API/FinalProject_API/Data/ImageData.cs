using FinalProject_API.ContextFolder;
using FinalProject_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace FinalProject_API.Data
{
    /// <summary>
    /// Репозиторий-бизнес логика приложения, 
    /// осуществляющая операции по
    /// взаимодействию с изображениями через БД
    /// </summary>
    public class ImageData
    {
        private DbContextOptions<DataContext> _options;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ServiceData"/> с инжекцией 
        /// зависимостей для <see cref="DbContextOptions{DataContext}"/>
        /// </summary>
        /// <param name="options">Экземпляр <see cref="DbContextOptions{DataContext}"/>, 
        /// используемый для для взаимодействия с базой данных</param>
        public ImageData(DbContextOptions<DataContext> options)
        {
            _options = options;
        }


        #region NoDB_Image
        /// <summary>
        /// Получение изображения из сетевой папки wwwroot в
        /// виде массива байтов
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<byte[]> GetNoDBImage(string fileName)
        {
            var filePath = Path.Combine("wwwroot", "images", fileName);
            if (!File.Exists(filePath))
            {
                return null;
            }
            byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
            return imageBytes;
        }

        /// <summary>
        /// Изменение изображения из сетевой папки wwwroot
        /// в виде массива байтов
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        public async Task<bool> ChangeNoDBImage(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                Console.WriteLine("Получен массив байтов, размер: " + imageFile.Length);
                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    var imageBytesArray = ms.ToArray();

                    using (var imageStream = new MemoryStream(imageBytesArray))
                    {
                        using (var image = Image.FromStream(imageStream))
                        {
                            var uniqueFileName = imageFile.FileName;
                            var filePath = Path.Combine("wwwroot", "images", uniqueFileName);
                            image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                            Console.WriteLine("Изображение сохранено по пути: " + filePath);
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region DB_Image
        /// <summary>
        /// Добавление изображения в БД
        /// </summary>
        /// <param name="imgModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task AddToDBImage(ImageModel imgModel, DataContext context)
        {
            await context.Images.AddAsync(imgModel);
        }

        /// <summary>
        /// Получение изображения из БД в виде массива байтов
        /// </summary>
        /// <param name="type"></param>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<byte[]> GetImageFromDB(string type, string uniqueName)
        {
            using (var context = new DataContext(_options))
            {
                var imgModel = await context.Images
                .Where(p => p.TypeOfModel == type)
                .Where(q => q.UniqueName == uniqueName)
                .FirstOrDefaultAsync();
                if (imgModel == null)
                {
                    throw new Exception($"Объект {uniqueName} отсутствует в БД");
                }
                return imgModel.ImageByteArray;
            }
        }

        /// <summary>
        /// Изменение изображения, находящегося в БД
        /// </summary>
        /// <param name="imgModel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ChangeDBImage(ImageModel imgModel, DataContext context)
        {
            ImageModel? img = await context.Images.FirstOrDefaultAsync(p => p.UniqueName == imgModel.UniqueName);
            if (img != null)
            {
                img.ImageByteArray = imgModel.ImageByteArray;
            }
        }
        #endregion
    }
}
