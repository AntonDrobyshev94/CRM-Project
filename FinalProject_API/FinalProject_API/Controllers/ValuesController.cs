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
    /// Контроллер, осуществляющий взаимодействие с заявками
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ApplicationData _repositoryData;
        private readonly TemporaryDatabase _temporaryDatabase;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ValuesController"/> с инжекцией 
        /// зависимостей для <see cref="ApplicationData"/>.
        /// </summary>
        /// <param name="repositoryData">Экземпляр <see cref="ApplicationData"/>, 
        /// используемый для взаимодействия с заявками.</param>
        public ValuesController(ApplicationData repositoryData,
            TemporaryDatabase temporaryDatabase)
        {
            _repositoryData = repositoryData;
            _temporaryDatabase = temporaryDatabase;
        }

        #region Application
        /// <summary>
        /// GET метод, возвращающий информацию о заявках
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<IApplication>> GetApplications()
        {
            return await _repositoryData.GetApplications();
        }

        /// <summary>
        /// POST запрос добавления новой заявки в базу данных
        /// </summary>
        /// <param name="value">Модель заявки</param>
        [HttpPost]
        [IdempotencyKey]
        public async Task<IActionResult> AddRequest([FromBody] Application value)
        {
            await Task.Delay(2000);
            string idempotencyKey = HttpContext.Items["IdempotencyKey"] as string;
            if (_temporaryDatabase.Get(idempotencyKey) == null)
            {
                Console.WriteLine($"Id {idempotencyKey} отсутствует, добавляем ");
                var result = await _repositoryData.AddApplications(value);
                _temporaryDatabase.AddOrUpdate(idempotencyKey, result);
                if ((result as StatusCodeResult)?.StatusCode == 200)
                {
                    return Ok(new { Message = $"Заявка {value.Name} c Id {value.Id} успешно добавлена" });
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
        /// POST запрос с атрибутом авторизации с ролью администратора
        /// Производит удаление заявки из базы данных
        /// </summary>
        /// <param name="id">Идентификатор удаляемой заявки</param>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task Delete(int id)
        {
            await _repositoryData.DeleteApplication(id);
        }

        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора
        /// Метод для изменения статуса заявки по указанному ID
        /// </summary>
        /// <param name="application">Модель заявки</param>
        [HttpPost]
        [Route("ChangeStatus")]
        [Authorize(Policy = "AdminOnly")]
        public async Task ChangeStatusApplication(Application application)
        {
            await _repositoryData.ChangeStatus(application);
        }
        #endregion
    }
}
