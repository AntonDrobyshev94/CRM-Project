using System.ComponentModel.DataAnnotations;

namespace FinalProject_Web.AuthFinalProjectApp
{
    public class UserRegistration
    {
        [Required (ErrorMessage = "Ошибка ввода логина. Проверьте заполнение поля.")]
        [MaxLength(20, ErrorMessage = "Длина логина должна быть не более 20 символов")]
        [MinLength(4, ErrorMessage = "Длина логина должна быть не менее 4 символов")]
        public string? LoginProp { get; set; }

        [Required (ErrorMessage = "Ошибка ввода пароля. Проверьте, заполнение поля.")]
        [MinLength(4, ErrorMessage = "Минимальная длина пароля - 4 символа")]
        [MaxLength(20, ErrorMessage = "Максимальная длина пароля - 20 символов")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage ="Пароли не совпадают")]
        public string? ConfirmPassword { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RequestId { get; set; }
    }
}
