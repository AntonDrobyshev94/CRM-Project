using Models.DataServiceModels;

namespace BusinessLogicService.Interfaces
{
    public interface ILinkData
    {
        #region Links
        Task<IEnumerable<LinkModel>> GetLinks(string url);
        Task AddLink(CommonWithImageModel<LinkModel> link, string token, string url);
        Task ChangeLink(CommonWithImageModel<LinkModel> link, string token, string url);
        Task DeleteLink(string token, string url);
        Task<LinkModel> FindLinkById(string token, string url);
        #endregion
    }
}
