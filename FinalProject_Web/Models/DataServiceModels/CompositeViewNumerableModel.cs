using Models.DataServiceModels.Interfaces;

namespace Models.DataServiceModels
{
    public class CompositeViewNumerableModel<T> : BaseViewModel<IEnumerable<T>>
    {}
}
