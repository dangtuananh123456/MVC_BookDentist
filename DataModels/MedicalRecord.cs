using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModels
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int SequenceNumber { get; set; }
        public DateOnly ExaminationDate { get; set; }
        public string Service { get; set; }
        public decimal ServicePrice { get; set; }
        public string Status { set; get; } = "no";

        [ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey(nameof(CreatedByDentist))]
        public string CreatedByDentistId { get; set; }
        public Dentist CreatedByDentist { get; set; }

        [ForeignKey(nameof(ExamDentist))]
        public string ExamDentistId { get; set; }
        public Dentist ExamDentist { get; set; }

        public ICollection<Medicine_MedicalRecord> Medicine_MedicalRecords { get; set; }
    }
}
