using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class RegisterProjectModel
    {
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Код")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Статус")]
        public int Status { get; set; }
    }
}