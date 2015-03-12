using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class RegisterUserModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "ФИО")]
        public string FIO { get; set; }

        [Required]
        [Display(Name = "Статус")]
        public int Status { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public int Role { get; set; }
    }
}