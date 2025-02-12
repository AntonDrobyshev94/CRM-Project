using FinalProject_API.Attributes;
using FinalProject_API.Data;
using FinalProject_API.Models;
using FinalProject_API.Models.Interfaces;
using FinalProject_API.Temporary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_API.Controllers
{
    /// <summary>
    /// Контроллер, осуществляющий взаимодействие с блогом
    /// </summary>
    [Route ("api/[controller]")]
    [ApiController]
    public class BlogController : Controller
    {
        private readonly BlogData _blogData;
        private readonly TemporaryDatabase _temporaryDatabase;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BlogController"/> с инжекцией 
        /// зависимостей для <see cref="BlogData"/>.
        /// </summary>
        /// <param name="blogData">Экземпляр <see cref="BlogData"/>, 
        /// используемый для взаимодействия с данными блога.</param>
        /// <param name="temporaryDatabase">Экземпляр <see cref="TemporaryDatabase"/>, 
        /// используемый для взаимодействия с временными данными (кэш).</param>
        public BlogController(BlogData blogData,
            TemporaryDatabase temporaryDatabase)
        {
            _blogData = blogData;
            _temporaryDatabase = temporaryDatabase;
        }

        #region Blog
        /// <summary>
        /// GET метод, возвращающий информацию о блоге
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<IBlogModel>> GetBlog()
        {
            return await _blogData.GetBlog();
        }

        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора.
        /// Метод добавления записи в блоге в базу данных
        /// </summary>
        /// <param name="blog">Объединённая модель блога и изображения</param>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [IdempotencyKey]
        public async Task<IActionResult> AddBlog([FromBody] CommonWithImageModel<BlogModel> blog)
        {
            await Task.Delay(2000);
            string idempotencyKey = HttpContext.Items["IdempotencyKey"] as string;
            if (_temporaryDatabase.Get(idempotencyKey) == null)
            {
                Console.WriteLine($"Ключ {idempotencyKey} отсутствует, добавляем ");
                var result = await _blogData.AddBlog(blog);
                _temporaryDatabase.AddOrUpdate(idempotencyKey, result);
                if ((result as StatusCodeResult)?.StatusCode == 200)
                {
                    return Ok(new { Message = $"Блог {blog.CommonModel.Name} c Id {blog.CommonModel.Id} успешно добавлен" });
                }
                else
                {
                    return result;
                }
            }
            else
            {
                Console.WriteLine($"Ключ {idempotencyKey} уже существует, направляем ответ");
                var response = _temporaryDatabase.Get(idempotencyKey);
                return Ok(response);
            }
        }

        /// <summary>
        /// Post метод с атрибутом авторизации с ролью администратора, 
        /// изменяющий блог
        /// </summary>
        /// <param name="blog">Объединённая модель блога и изображения</param>
        [HttpPost]
        [Route("ChangeBlog")]
        [Authorize(Policy = "AdminOnly")]
        [IdempotencyKey]
        public async Task<IActionResult> ChangeBlog(CommonWithImageModel<BlogModel> blog)
        {
            await Task.Delay(2000);
            string idempotencyKey = HttpContext.Items["IdempotencyKey"] as string;
            if (_temporaryDatabase.Get(idempotencyKey) == null)
            {
                Console.WriteLine($"Id {idempotencyKey} отсутствует, добавляем ");
                var result = await _blogData.ChangeBlog(blog);
                _temporaryDatabase.AddOrUpdate(idempotencyKey, result);
                if ((result as StatusCodeResult)?.StatusCode == 200)
                {
                    return Ok(new { Message = $"Блог {blog.CommonModel.Name} c Id {blog.CommonModel.Id} успешно добавлен" });
                }
                else
                {
                    return result;
                }
            }
            else
            {
                Console.WriteLine($"Id {idempotencyKey} уже существует, направляем ответ");
                var response = _temporaryDatabase.Get(idempotencyKey);
                return Ok(response);
            }
        }

        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора
        /// Метод удаления блога из базы данных
        /// </summary>
        /// <param name="id">Идентификатор удаляемого блога</param>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task DeleteBlog(int id)
        {
            await _blogData.DeleteBlog(id);
        }

        /// <summary>
        /// Get Асинхронный метод, предоставляющий информацию о
        /// выбранном блоге (возвращающий экземпляр BlogModel)
        /// </summary>
        /// <param name="id">Идентификатор вызываемого блога</param>
        [HttpGet("{id}")]
        public async Task<BlogModel> BlogDetails(int id)
        {
            return await _blogData.GetBlogByID(id);
        }
        #endregion
    }
}
