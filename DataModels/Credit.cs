using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels
{
    [Table("Credits")]
    public class Credit
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(customer))]
        public string CustomerId { get; set; }
        public Customer customer { get; set; }
        public decimal Balance { set; get; }
    }
}
