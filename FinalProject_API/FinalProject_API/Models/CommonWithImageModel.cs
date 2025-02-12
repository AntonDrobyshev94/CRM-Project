using FinalProject_API.Models.Interfaces;

namespace FinalProject_API.Models
{
    public class CommonWithImageModel<T> where T : ICommon
    {
        public T CommonModel { get; set; }
        public ImageModel ImgModel { get; set; }
    }
}
