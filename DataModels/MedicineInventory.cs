using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels
{
    public class MedicineInventory
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Medicine))]
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public int InventoryQuantity { get; set; }
        public string? Unit { get; set; }
    }
}
