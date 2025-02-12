namespace FinalProject_Web.Services.Interfaces
{
    public interface ILayoutViewModelServiceCreate <T>
    {
        public Task<T> Create(bool isRedact);
    }
}
