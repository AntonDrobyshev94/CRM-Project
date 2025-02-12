using Models.DataServiceModels.Interfaces;

namespace Models.DataServiceModels
{
    public class Service : ICommon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
