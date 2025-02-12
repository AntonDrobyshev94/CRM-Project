namespace FinalProject_API.Models.Interfaces
{
    public interface IBlogModel
    {
        int Id { get; set; }
        string ImageName { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string BlogPost { get; set; }
        DateTime DateTimePublication { get; set; }
    }
}
