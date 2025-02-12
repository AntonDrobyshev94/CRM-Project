using FinalProject_Web.Model.Interfaces;
using FinalProject_Web.Model;

namespace FinalProject_Web.AuthFinalProjectApp
{
    public class RegistrationViewModel : IBaseViewModel
    {
        public LayoutViewModel LayoutViewModel { get; set; }
        public UserRegistration UserRegistration { get; set; }
    }
}
