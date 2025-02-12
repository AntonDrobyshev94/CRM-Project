using FinalProject_API.Models.Interfaces;

namespace FinalProject_API.Models
{
    public class TagModel : ITagModel
    {
        public int Id { get; set; }
        public string Tag { get; set; }
    }
}
