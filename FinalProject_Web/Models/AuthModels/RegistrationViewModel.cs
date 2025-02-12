using Models.DataServiceModels.Interfaces;
using Models.DataServiceModels;

namespace Models.AuthModels
{
    public class RegistrationViewModel : IBaseViewModel
    {
        public LayoutViewModel LayoutViewModel { get; set; }
        public UserRegistration UserRegistration { get; set; }
    }
}
