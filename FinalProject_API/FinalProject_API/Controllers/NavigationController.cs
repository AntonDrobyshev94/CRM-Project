using FinalProject_API.Data;
using FinalProject_API.Models;
using FinalProject_API.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_API.Controllers
{
    /// <summary>
    /// Контроллер, осуществляющий взаимодействие с параметрами
    /// меню навигации
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NavigationController : Controller
    {
        private readonly MenuNavigationAndTagData _navigationAndTagData;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="NavigationController"/> с инжекцией 
        /// зависимостей для <see cref="MenuNavigationAndTagData"/>.
        /// </summary>
        /// <param name="navigationAndTagData">Экземпляр <see cref="MenuNavigationAndTagData"/>, 
        /// используемый для взаимодействия с данными навигационного меню.</param>
        public NavigationController(MenuNavigationAndTagData navigationAndTagData)
        {
            _navigationAndTagData = navigationAndTagData;
        }

        #region Title
        /// <summary>
        /// GET метод, возвращающий информацию о титульной странице
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<TitleModel> GetTitle()
        {
            return await _navigationAndTagData.GetTitle();
        }

        /// <summary>
        /// Post метод с атрибутом авторизации с ролью администратора, 
        /// изменяющий проект
        /// </summary>
        /// <param name="title">Модель титульного меню</param>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task ChangeTitle(TitleModel title)
        {
            await _navigationAndTagData.ChangeTitle(title);
        }
        #endregion

        #region Tag

        /// <summary>
        /// GET метод, возвращающий случайный тэг
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRandomTag")]
        public async Task<ITagModel> GetRandomTagMethod()
        {
            return await _navigationAndTagData.GetRandomTag();
        }

        /// <summary>
        /// GET метод, возвращающий информацию о тэгах
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTags")]
        public async Task<IEnumerable<ITagModel>> GetTagsMethod()
        {
            return await _navigationAndTagData.GetTags();
        }

        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора.
        /// Метод добавления нового тэга в базу данных
        /// </summary>
        /// <param name="tag">Модель тэга</param>
        [HttpPost]
        [Route("AddTag")]
        [Authorize(Policy = "AdminOnly")]
        public async Task AddTagMethod([FromBody] TagModel tag)
        {
            await _navigationAndTagData.AddTag(tag);
        }

        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора
        /// Метод удаления тэга из базы данных
        /// </summary>
        /// <param name="id">Идентификатор удаляемого тэга</param>
        [HttpDelete]
        [Route("DeleteTag/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task DeleteTag(int id)
        {
            await _navigationAndTagData.DeleteTag(id);
        }
        #endregion
    }
}
