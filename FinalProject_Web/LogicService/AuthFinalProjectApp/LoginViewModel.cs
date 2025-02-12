using FinalProject_Web.Model;
using FinalProject_Web.Model.Interfaces;

namespace FinalProject_Web.AuthFinalProjectApp
{
    public class LoginViewModel : IBaseViewModel
    {
        public LayoutViewModel LayoutViewModel { get; set; }
        public UserLogin UserLogin { get; set; }
    }
}
