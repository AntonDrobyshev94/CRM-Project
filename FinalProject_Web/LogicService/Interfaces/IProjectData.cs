using FinalProject_Web.Model;

namespace FinalProject_Web.Interfaces
{
    public interface IProjectData
    {
        #region Projects
        Task<IEnumerable<ProjectModel>> GetProjects();
        Task AddProject(CommonWithImageModel<ProjectModel> project, HttpContext httpContext);
        Task ChangeProject(CommonWithImageModel<ProjectModel> project, HttpContext httpContext);
        Task DeleteProject(int id, HttpContext httpContext);
        Task<ProjectModel> FindProjectById(int id, HttpContext httpContext);
        #endregion
    }
}
