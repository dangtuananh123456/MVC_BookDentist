using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class EditProfileModel
    {
        [BindNever]
        [ValidateNever]
        public string Id { get; set; }
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DayOfBirth { get; set; }
    }
}
