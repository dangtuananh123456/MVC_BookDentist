using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
	public class NumberMedicine
	{
		public int medicineId {  get; set; }
		public int quantity { set; get; }
	}
	public class FormPrescriptionModel
	{
		public int id { set; get; }
		public int sequence { set; get; }
		public List<NumberMedicine>? numberMedicines { set; get;}
	}
}
