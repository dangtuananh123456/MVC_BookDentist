using DataModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class MedicalRecordModel
    {
        [Display(Name = "Ngày khám")]
        [DataType(DataType.Date)]
        public DateTime ExaminationDate { get; set; }
        [Display(Name = "Dịch vụ")]
        public string Service { get; set; }
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
        public string CreatedByDentistId { get; set; }
    }
}
