namespace Models.DataServiceModels.Interfaces
{
    public abstract class BaseViewModel<T> : IBaseViewModel
    {
        public LayoutViewModel LayoutViewModel { get; set; }
        public T ContentViewModel { get; set; }
    }
}
