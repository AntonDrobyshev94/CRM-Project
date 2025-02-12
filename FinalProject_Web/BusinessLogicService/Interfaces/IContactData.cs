using ServicesLibrary.Enums;
using Models.DataServiceModels;

namespace BusinessLogicService.Interfaces
{
    public interface IContactData
    {
        #region Contacts
        Task<Contacts> GetContacts(string url);
        Task<ChangeContactsResult> ChangeContacts(Contacts contacts, string token, string url);
        #endregion
    }
}
