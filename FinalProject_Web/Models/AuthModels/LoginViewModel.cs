using Models.DataServiceModels.Interfaces;
using Models.DataServiceModels;

namespace Models.AuthModels
{
    public class LoginViewModel : IBaseViewModel
    {
        public LayoutViewModel LayoutViewModel { get; set; }
        public UserLogin UserLogin { get; set; }
    }
}
