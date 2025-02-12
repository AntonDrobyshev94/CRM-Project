using Models.DataServiceModels;

namespace BusinessLogicService.Interfaces
{
    public interface IApplicationData
    {
        #region Applications
        Task<IEnumerable<Application>> GetApplications(string token, string url);
        Task AddApplication(Application application, string token, string url, string requestId);
        Task DeleteApplication(string token, string url);
        Task ChangeApplicationStatus(string status, int id, string token, string url);
        #endregion
    }
}
