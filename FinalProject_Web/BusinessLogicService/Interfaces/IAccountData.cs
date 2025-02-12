using Models.AuthModels;

namespace BusinessLogicService.Interfaces
{
    public interface IAccountData
    {
        #region Authentication
        Task<string> IsRegist(UserRegistration model, string url);
        Task<string> IsLogin(UserLoginProp model, string url);
        #endregion
    }
}
