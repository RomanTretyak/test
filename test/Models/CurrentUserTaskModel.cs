using System;
using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class CurrentUserTaskModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [AtributeMaxDate(ErrorMessage = "Дата не может быть больше текущей")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата")]
        public DateTime? CreateDate { get; set; }

        [Required]
        [Display(Name = "Задача")]
        public string Task { get; set; }

        [Required]
        [Display(Name = "Время")]
        [AtributeMinTime(ErrorMessage = "Значение не может быть меньше 5")]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public int? Time { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public string StatusName { get; set; }

        [Required]
        [Display(Name = "Статус")]
        public int Status { get; set; }

    }
}