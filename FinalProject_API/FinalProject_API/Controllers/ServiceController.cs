using FinalProject_API.Data;
using FinalProject_API.Models;
using FinalProject_API.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_API.Controllers
{
    /// <summary>
    /// Контроллер, осуществляющий взаимодействие с услугами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : Controller
    {
        private readonly ServiceData _serviceData;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ServiceController"/> с инжекцией 
        /// зависимостей для <see cref="ServiceData"/>.
        /// </summary>
        /// <param name="serviceData">Экземпляр <see cref="AccountData"/>, 
        /// используемый для взаимодействия с данными предоставляемых услуг.</param>
        public ServiceController(ServiceData serviceData)
        {
            _serviceData = serviceData;
        }

        #region Services
        /// <summary>
        /// GET метод, возвращающий информацию об услугах
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<IService>> GetServices()
        {
            return await _serviceData.GetServices();
        }

        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора.
        /// Метод добавления новой услуги в базу данных
        /// </summary>
        /// <param name="service">Модель услуги</param>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task AddService([FromBody] Service service)
        {
            await _serviceData.AddService(service);
        }

        /// <summary>
        /// Post метод с атрибутом авторизации с ролью администратора, 
        /// изменяющий услугу
        /// </summary>
        /// <param name="service">Модель услуги</param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangeService")]
        [Authorize(Policy = "AdminOnly")]
        public async Task ChangeService([FromBody] Service service)
        {
            await _serviceData.ChangeService(service);
        }

        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора
        /// удаления услуги из базы данных
        /// </summary>
        /// <param name="id">Идентификатор удаляемой услуги</param>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task DeleteService(int id)
        {
            await _serviceData.DeleteService(id);
        }

        /// <summary>
        /// Get Асинхронный метод, предоставляющий информацию о
        /// выбранной услуге
        /// </summary>
        /// <param name="id">Идентификатор запрашиваемой услуги</param>
        [HttpGet("{id}")]
        public async Task<Service> FindServiceById(int id)
        {
            return await _serviceData.GetServiceByID(id);
        }
        #endregion
    }
}
