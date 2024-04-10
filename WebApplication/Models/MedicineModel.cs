using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class MedicineModel
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Prescription { get; set; }
        [Required]
        public decimal? Price { get; set; } = 2000;
    }
}