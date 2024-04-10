using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{

	public class MyPrescriptionMedicine
	{
		public Medicine_MedicalRecord medicine_MedicalRecord { set; get; }
		public Medicine medicine { set; get; }
	}
	public class InformationPrescriptionModel
	{
		public Customer customer { set; get; }
		public MedicalRecord medicalRecord { set; get; }
		public List<MyPrescriptionMedicine>? listMedicine { set; get; }
	}
}
