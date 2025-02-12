using Models.DataServiceModels;

namespace BusinessLogicService.Interfaces
{
    public interface INavigationData
    {
        #region Tag
        Task AddTagMethod(TagModel tag, string token, string url);
        Task<TagModel> GetRandomTag(string url);
        Task<IEnumerable<TagModel>> GetTags(string url);
        Task DeleteTag(string token, string url);
        #endregion
        #region Tittle
        Task<TitleModel> GetTitle(string url);
        Task ChangeTitle(TitleModel tittleModel, string token, string url);
        TitleModel EditTitle(string newTittle, string mainTittle,
            string servicesTittle, string projectsTittle,
            string blogTittle, string contactsTittle);
        #endregion
    }
}
