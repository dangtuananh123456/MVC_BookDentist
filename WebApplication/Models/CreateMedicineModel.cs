using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class CreateMedicineModel
    {
        public int Id { get; set; }
        [Display(Name = "Tên thuốc")]  
        public string Name { get; set; }
        [Display(Name = "Chỉ định")]
        public string? Prescription { get; set; }
        [Display(Name = "Giá")]
        public decimal? Price { get; set; }
    }
}