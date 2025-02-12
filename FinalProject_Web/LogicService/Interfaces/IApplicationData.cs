using FinalProject_Web.AuthFinalProjectApp;
using FinalProject_Web.Model;
using System.Net;

namespace FinalProject_Web.Interfaces
{
    public interface IApplicationData
    {
        #region Applications
        Task<IEnumerable<Application>> GetApplications(HttpContext httpContext);
        Task AddApplication(Application application, HttpContext httpContext);
        Task DeleteApplication(int id, HttpContext httpContext);
        Task ChangeApplicationStatus(string status, int id, HttpContext httpContext);
        #endregion
    }
}
