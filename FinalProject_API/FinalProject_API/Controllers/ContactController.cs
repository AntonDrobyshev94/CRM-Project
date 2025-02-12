using FinalProject_API.Data;
using FinalProject_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_API.Controllers
{
    /// <summary>
    /// Контроллер, осуществляющий взаимодействие с контактами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : Controller
    {
        private readonly ContactData _contactData;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ContactController"/> с инжекцией 
        /// зависимостей для <see cref="ContactData"/>.
        /// </summary>
        /// <param name="contactData">Экземпляр <see cref="ContactData"/>, 
        /// используемый для взаимодействия с данными контактов.</param>
        public ContactController(ContactData contactData)
        {
            _contactData = contactData;
        }

        #region Contacts
        /// <summary>
        /// GET метод, возвращающий информацию о контактах
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Contacts> GetContacts()
        {
            return await _contactData.GetContacts();
        }

        /// <summary>
        /// Post метод с атрибутом авторизации с ролью администратора, 
        /// изменяющий контакты
        /// </summary>
        /// <param name="contacts">Модель контактов</param>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ChangeContacts(Contacts contacts)
        {
            bool isChangeContacts = await _contactData.ChangeContacts(contacts);
            if (isChangeContacts)
            {
                return Ok("Контакты сохранены");
            }
            else
            {
                return BadRequest("Контакты не удалось изменить.");
            }
        }
        #endregion
    }
}
