using FinalProject_Web.Model;

namespace FinalProject_Web.Interfaces
{
    public interface ILinkData
    {
        #region Links
        Task<IEnumerable<LinkModel>> GetLinks();
        Task AddLink(CommonWithImageModel<LinkModel> link, HttpContext httpContext);
        Task ChangeLink(CommonWithImageModel<LinkModel> link, HttpContext httpContext);
        Task DeleteLink(int id, HttpContext httpContext);
        Task<LinkModel> FindLinkById(int id, HttpContext httpContext);
        #endregion
    }
}
