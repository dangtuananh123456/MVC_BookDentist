using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModels
{
    public class Dentist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public AppUser Account { get; set; }
        [ForeignKey(nameof(Account))]
        public string AccountId { get; set; }
    }
}
