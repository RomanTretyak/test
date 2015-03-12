using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class JoinUsersToProject
    {
        [Required]
        [Display(Name = "Пользователь")]
        public int User { get; set; }

        [Required]
        [Display(Name = "Проект")]
        public int Project { get; set; }
    }
}