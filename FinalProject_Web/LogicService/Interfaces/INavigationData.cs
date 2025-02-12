using FinalProject_Web.Model;

namespace FinalProject_Web.Interfaces
{
    public interface INavigationData
    {
        void EditMode(HttpContext context);
        #region Tag
        Task AddTagMethod(TagModel tag, HttpContext httpContext);
        Task<TagModel> GetRandomTag();
        Task<IEnumerable<TagModel>> GetTags();
        Task DeleteTag(int id, HttpContext httpContext);
        #endregion
        #region Tittle
        Task<TitleModel> GetTitle();
        Task ChangeTitle(TitleModel tittleModel, HttpContext httpContext);
        TitleModel EditTitle(string newTittle, string mainTittle,
            string servicesTittle, string projectsTittle,
            string blogTittle, string contactsTittle);
        #endregion
    }
}
