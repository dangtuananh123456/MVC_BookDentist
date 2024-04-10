using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class PhoneModel
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}
