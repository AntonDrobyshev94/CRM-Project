using System.ComponentModel.DataAnnotations;

namespace Models.AuthModels
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Ошибка ввода логина. Проверьте заполнение поля.")]
        [MaxLength(20, ErrorMessage = "Длина строки должна быть не более 20 символов.")]
        [MinLength(4, ErrorMessage = "Длина строки должна быть не менее 4 символов.")]
        public string? LoginProp { get; set; }

        [Required(ErrorMessage = "Ошибка ввода пароля. Проверьте, заполнение поля."), DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Минимальная длина пароля - 4 символа")]
        [MaxLength(20, ErrorMessage = "Максимальная длина пароля - 20 символов")]
        public string? Password { get; set; }
    }
}
