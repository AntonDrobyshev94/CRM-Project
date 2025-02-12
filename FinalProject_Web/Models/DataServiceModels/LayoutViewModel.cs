using System.ComponentModel.DataAnnotations;

namespace Models.DataServiceModels
{
    public class LayoutViewModel
    {
        public TitleModel TitleModel { get; set; }
        public TagModel TagModel { get; set; }
        public bool IsEditMode { get; set; }
        public bool IsAdmin { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UserName { get; set; }
        public bool IsAuth { get; set; }
        public bool IsRedactWindow { get; set; }
        public string RedactButtonText => IsEditMode ? "Закончить редактирование" : "Редактировать";
    }
}
