using FinalProject_Web.Model;

namespace FinalProject_Web.Interfaces
{
    public interface IBlogData
    {
        #region Blog
        Task<IEnumerable<BlogModel>> GetBlog();
        Task AddBlog(CommonWithImageModel<BlogModel> blog, HttpContext httpContext);
        Task ChangeBlog(CommonWithImageModel<BlogModel> blog, HttpContext httpContext);
        Task DeleteBlog(int id, HttpContext httpContext);
        Task<BlogModel> FindBlogById(int id, HttpContext httpContext);
        #endregion
    }
}
