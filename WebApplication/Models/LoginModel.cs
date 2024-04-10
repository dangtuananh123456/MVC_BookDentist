using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class LoginModel
    {
        [Display(Name = "Tên đăng nhập (Số điện thoại đối với khách hàng)")]
		[Required(ErrorMessage = "Tên đăng nhập không được để trống")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		[DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
