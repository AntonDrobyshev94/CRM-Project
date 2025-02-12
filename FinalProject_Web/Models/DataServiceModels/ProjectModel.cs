using Models.DataServiceModels.Interfaces;

namespace Models.DataServiceModels
{
    public class ProjectModel : ICommon
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Name { get; set; }   
        public string Description { get; set; }
    }
}
