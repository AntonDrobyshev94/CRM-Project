using FinalProject_API.Data;
using FinalProject_API.Models;
using FinalProject_API.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_API.Controllers
{
    /// <summary>
    /// Контроллер, осуществляющий взаимодействие с проектами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : Controller
    {
        private readonly ProjectData _projectData;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ProjectController"/> с инжекцией 
        /// зависимостей для <see cref="ProjectData"/>.
        /// </summary>
        /// <param name="projectData">Экземпляр <see cref="ProjectData"/>, 
        /// используемый для взаимодействия с данными проектов.</param>
        public ProjectController(ProjectData projectData)
        {
            _projectData = projectData;
        }

        #region Projects
        /// <summary>
        /// GET метод, возвращающий информацию о проектах
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<IProjectModel>> GetProjects()
        {
            return await _projectData.GetProjects();
        }

        /// <summary>
        /// Post метод с атрибутом авторизации с ролью администратора, 
        /// принимающий модель проекта и изменяющий проект
        /// </summary>
        /// <param name="project">Модель проекта</param>
        [HttpPost]
        [Route("ChangeProject")]
        [Authorize(Policy = "AdminOnly")]
        public async Task ChangeProject(CommonWithImageModel<ProjectModel> project)
        {
            await _projectData.ChangeProjects(project);
        }

        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора
        /// Метод добавления нового проекта в базу данных
        /// </summary>
        /// <param name="project">Модель проекта</param>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task AddProject([FromBody] CommonWithImageModel<ProjectModel> project)
        {
            await _projectData.AddProjects(project);
        }
        /// <summary>
        /// POST запрос с атрибутом авторизации с ролью администратора
        /// Метод удаления проекта из базы данных
        /// </summary>
        /// <param name="id">Идентификатор удаляемого проекта</param>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task DeleteProject(int id)
        {
            await _projectData.DeleteProject(id);
        }

        /// <summary>
        /// Get Асинхронный метод, предоставляющий информацию о
        /// выбранном проекте
        /// </summary>
        /// <param name="id">Идетнификатор запрашиваемого проекта</param>
        [HttpGet("{id}")]
        public async Task<ProjectModel> ProjectDetails(int id)
        {
            return await _projectData.GetProjectByID(id);
        }
        #endregion
    }
}
