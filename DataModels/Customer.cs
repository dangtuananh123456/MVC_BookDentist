using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels
{
	public class Customer
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime DayOfBirth { get; set; }
        public AppUser Account { get; set; }
        [ForeignKey(nameof(Account))]
        public string AccountId { get; set; }
        public Credit credit { set; get; }
    }
}
