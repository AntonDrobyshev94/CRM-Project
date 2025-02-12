using Models.DataServiceModels.Interfaces;

namespace Models.DataServiceModels
{
    public class LinkModel : ICommon
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Url { get; set; }
    }
}
