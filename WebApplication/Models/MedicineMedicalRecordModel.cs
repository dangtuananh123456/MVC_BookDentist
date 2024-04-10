using DataModels;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class MedicineMedicalRecordModel
    {
        [Required]
        public int MedicineId { get; set; }
        [Required]
        public int MedicalRecordId { get; set; }
        [Required]
        public int SequenceNumber { get; set; }
        [Required]
        public int MedicineQuantity { get; set; }
    }
}
