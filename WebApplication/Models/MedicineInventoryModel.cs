using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class MedicineInventoryModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Thuốc")]
        public int MedicineId { get; set; }   
        [Required]
        [Display(Name = "Ngày hết hạn")]
        public DateOnly ExpiryDate { get; set; }
        [Required]
        [Display(Name = "Số lượng")]
        public int InventoryQuantity { get; set; }
        [Required]
        [Display(Name = "Đơn vị")]
        public string? Unit { get; set; } = "Viên";
    }
}