using FinalProject_API.Models.Interfaces;

namespace FinalProject_API.Models
{
    public class Service : IService, ICommon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
