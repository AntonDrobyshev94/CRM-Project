using FinalProject_API.Models.Interfaces;

namespace FinalProject_API.Models
{
    public class LinkModel : ILinkModel, ICommon
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Url { get; set; }
    }
}
