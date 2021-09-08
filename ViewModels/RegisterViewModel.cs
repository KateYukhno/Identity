using System.ComponentModel.DataAnnotations;

namespace AppIdentity.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Не указано имя пользователя")]
        [DataType(DataType.Text)]
        [MaxLength(255)]
        [Display(Name = "Имя пользователя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан логин")]
        [DataType(DataType.Text)]
        [MaxLength(255)]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 7)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required (ErrorMessage = "Не указан повторно пароль")]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string PasswordConfirmation { get; set; }
    }
}