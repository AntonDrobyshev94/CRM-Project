using Models.DataServiceModels.Interfaces;

namespace Models.DataServiceModels
{
    public class Contacts : ICommon
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Email { get;set; }
    }
}
