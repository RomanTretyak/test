using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class RegisterTaskModel
    {
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Статус")]
        public int Status { get; set; }

        [Required]
        [Display(Name = "Проект")]
        public int Project { get; set; }
    }
}