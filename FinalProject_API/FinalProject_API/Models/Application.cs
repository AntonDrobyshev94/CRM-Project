using FinalProject_API.Models.Interfaces;

namespace FinalProject_API.Models
{
    public class Application: IApplication, ICommon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}
