using DataModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
	public class CustomerModel
	{
		public string Id { get; set; }
		public string PhoneNumber { get; set; }
		public string FullName { get; set; }
		public string Address { get; set; }
		public DateTime DayOfBirth { get; set; }
		public string AccountId { get; set; }
	}
}
