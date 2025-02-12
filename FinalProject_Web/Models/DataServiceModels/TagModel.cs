using System.ComponentModel.DataAnnotations;

namespace Models.DataServiceModels
{
    public class TagModel
    {
        public int Id { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Tag { get; set; }
    }
}
