using FinalProject_Web.Enums;
using FinalProject_Web.Model;

namespace FinalProject_Web.Interfaces
{
    public interface IContactData
    {
        #region Contacts
        Task<Contacts> GetContacts();
        Task<ChangeContactsResult> ChangeContacts(Contacts contacts, HttpContext httpContext);
        #endregion
    }
}
