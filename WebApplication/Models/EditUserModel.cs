using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class EditUserModel
    {
        [Required]
        public string Id { get; set; }
        [Display(Name = "Tên đăng nhập")]

        public string UserName { get; set; }
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }
        public bool IsLocked { get; set; }
    }
}
