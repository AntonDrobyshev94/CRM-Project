using Models.DataServiceModels;

namespace BusinessLogicService.Interfaces
{
    public interface IServiceData
    {
        #region Services
        Task<IEnumerable<Service>> GetServices(string url);
        Task AddService(Service service, string token, string url);
        Task ChangeService(Service service, string token, string url);
        Task DeleteService(string token, string url);
        Task<Service> FindServiceById(string token, string url);
        #endregion
    }
}
