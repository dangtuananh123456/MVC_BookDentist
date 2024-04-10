using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
	public class EditServicePriceModel
	{
		[Required]
		public int Id { get; set; }
		[Required]
		public int Sequence { get; set; }
		[Required]
		public decimal Price { get; set; }
	}
}
