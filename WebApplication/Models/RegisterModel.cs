using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class RegisterModel
    {
        [Display(Name = "Số điện thoại")]
		[Required(ErrorMessage = "Số điện thoại không được để trống")]
		public string PhoneNumber { get; set; }
		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		[DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
		[Required(ErrorMessage = "Không được để trống")]
		[Compare(nameof(Password), ErrorMessage = "Không khớp với mật khẩu")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Họ và tên")]
		[Required(ErrorMessage = "Họ và tên không được để trống")]
		public string FullName { get; set; }
        [Display(Name = "Địa chỉ")]
		[Required(ErrorMessage = "Địa chỉ không được để trống")]
		public string Address { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
		[Required(ErrorMessage = "Ngày sinh không được để trống")]
		public DateTime DayOfBirth { get; set; }
	}
}
