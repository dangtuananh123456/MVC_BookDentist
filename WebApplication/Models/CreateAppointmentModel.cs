

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
	public class CreateAppointmentModel
	{
		[Display(Name = "Ngày hẹn")]
		[DataType(DataType.Date)]
		public DateTime Date { get; set; }
		[Display(Name = "Thời gian bắt đầu")]
		[DataType(DataType.Time)]
		public DateTime StartTime { get; set; }
		[Display(Name = "Thời gian kết thúc")]
		[DataType(DataType.Time)]
		public DateTime EndTime { get; set; }
		[Display(Name = "Chọn nha sĩ khám")]
		public string DentistId { get; set;}
		public string CustomerId { get; set;}
	}
}
