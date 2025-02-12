using Models.DataServiceModels.Interfaces;

namespace Models.DataServiceModels
{
    public class CommonWithImageModel<T> where T : ICommon 
    {
        public T CommonModel { get; set; }
        public ImageModel ImgModel { get; set; }
    }
}
