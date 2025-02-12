namespace FinalProject_API.Models.Interfaces
{
    public interface IProjectModel
    {
        int Id { get; set; }
        string ImageName { get; set; }
        string Name { get; set; }
        string Description { get; set; }
    }
}
