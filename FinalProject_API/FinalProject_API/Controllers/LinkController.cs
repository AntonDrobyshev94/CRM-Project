using FinalProject_API.Data;
using FinalProject_API.Models;
using FinalProject_API.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_API.Controllers
{
    /// <summary>
    /// Контроллер, осуществляющий взаимодействие со ссылками
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : Controller
    {
        private readonly LinkData _linkData;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LinkController"/> с инжекцией 
        /// зависимостей для <see cref="LinkData"/>.
        /// </summary>
        /// <param name="linkData">Экземпляр <see cref="LinkData"/>, 
        /// используемый для взаимодействия с данными ссылок.</param>
        public LinkController(LinkData linkData)
        {
            _linkData = linkData;
        }

        #region Links
        /// <summary>
        /// GET метод, возвращающий информацию о ссылках
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ILinkModel>> GetLinks()
        {
            return await _linkData.GetLinks();
        }

        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора.
        /// Метод добавления новой ссылки в базу данных
        /// </summary>
        /// <param name="link">Модель ссылки</param>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task AddLink([FromBody] CommonWithImageModel<LinkModel> link)
        {
            await _linkData.AddLink(link);
        }

        /// <summary>
        /// Post метод с атрибутом авторизации с ролью администратора, 
        /// изменяющий ссылку 
        /// </summary>
        /// <param name="link">Модель ссылки</param>
        [HttpPost]
        [Route("ChangeLink")]
        [Authorize(Policy = "AdminOnly")]
        public async Task ChangeLink(CommonWithImageModel<LinkModel> link)
        {
            await _linkData.ChangeLink(link);
        }

        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора
        /// Метод удаления ссылки из базы данных
        /// </summary>
        /// <param name="id">Идентификатор удаляемой ссылки</param>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task DeleteLink(int id)
        {
            await _linkData.DeleteLink(id);
        }

        /// <summary>
        /// Get Асинхронный метод с атрибутом авторизации с ролью 
        /// администратора, предоставляющий экземпляр запрошенной 
        /// ссылки
        /// </summary>
        /// <param name="id">Идентификатор ссылки</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<LinkModel> LinkDetails(int id)
        {
            return await _linkData.GetLinkByID(id);
        }
        #endregion
    }
}
