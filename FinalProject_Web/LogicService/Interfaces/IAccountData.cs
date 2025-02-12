using FinalProject_Web.AuthFinalProjectApp;

namespace FinalProject_Web.Interfaces
{
    public interface IAccountData
    {
        #region Authentication
        Task<string> IsRegist(UserRegistration model);
        Task<string> IsLogin(UserLoginProp model);
        #endregion
    }
}
