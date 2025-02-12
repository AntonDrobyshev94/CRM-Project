using Models.DataServiceModels.Interfaces;

namespace Models.DataServiceModels
{
    public class SimpleViewModel : IBaseViewModel
    {
        public LayoutViewModel LayoutViewModel { get; set; }
        public IEnumerable<TagModel> Tags { get; set; }
    }
}
