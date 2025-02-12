using FinalProject_Web.Model;

namespace FinalProject_Web.Interfaces
{
    public interface IServiceData
    {
        #region Services
        Task<IEnumerable<Service>> GetServices();
        Task AddService(Service service, HttpContext httpContext);
        Task ChangeService(string name, string description,
            int id, HttpContext httpContext);
        Task DeleteService(int id, HttpContext httpContext);
        Task<Service> FindServiceById(int id, HttpContext httpContext);
        #endregion
    }
}
