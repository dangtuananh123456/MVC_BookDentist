using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class PersonalScheduleModel
    {
        [Display(Name = "Ngày")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Display(Name = "Bắt đầu lúc")]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [Display(Name = "Kết thúc lúc")]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
        public string DentistId { get; set; }
    }
}
