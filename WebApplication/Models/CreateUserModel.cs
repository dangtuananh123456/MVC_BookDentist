using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class CreateUserModel
    {
		[Display(Name = "Tên đăng nhập")]
		[Remote(action: "IsAccountAvailable", controller: "Account")]
		public string UserName { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Mật khẩu")]
		public string Password { get; set; }
		[DataType(DataType.Password)]
		[Display(Name = "Xác nhận mật khẩu")]
		[Compare(nameof(Password), ErrorMessage = "Không khớp với mật khẩu")]
		public string ConfirmPassword { get; set; }
		[Display(Name = "Họ và tên")]
		public string FullName { get; set; }
		[Display(Name = "Số điện thoại")]
		public string PhoneNumber { get; set; }
        [Display(Name = "Chức vụ")]
        public string UserType { get; set; }
	}
}
