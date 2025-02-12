using Models.DataServiceModels;

namespace BusinessLogicService.Interfaces
{
    public interface IBlogData
    {
        #region Blog
        Task<IEnumerable<BlogModel>> GetBlog(string url);
        Task AddBlog(CommonWithImageModel<BlogModel> blog, string token, string url, string requsetId);
        Task ChangeBlog(CommonWithImageModel<BlogModel> blog, string token, string url, string requsetId);
        Task DeleteBlog(string token, string url);
        Task<BlogModel> FindBlogById(string token, string url);
        #endregion
    }
}
