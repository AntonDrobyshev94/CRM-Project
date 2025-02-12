using Models.DataServiceModels;

namespace BusinessLogicService.Interfaces
{
    public interface IProjectData
    {
        #region Projects
        Task<IEnumerable<ProjectModel>> GetProjects(string url);
        Task AddProject(CommonWithImageModel<ProjectModel> project, string token, string url);
        Task ChangeProject(CommonWithImageModel<ProjectModel> project, string token, string url);
        Task DeleteProject(string token, string url);
        Task<ProjectModel> FindProjectById(string token, string url);
        #endregion
    }
}
