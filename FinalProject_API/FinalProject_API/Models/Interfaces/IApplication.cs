namespace FinalProject_API.Models.Interfaces
{
    public interface IApplication
    {
        int Id { get; set; }
        string Name { get; set; }
        string EMail { get; set; }
        string Message { get; set; }
        string Status { get; set; }
        DateTime Date { get; set; }
    }
}
