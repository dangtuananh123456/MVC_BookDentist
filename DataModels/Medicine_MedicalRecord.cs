namespace DataModels
{
    public class Medicine_MedicalRecord
    {
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }
        public int MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
        public int SequenceNumber { get; set; }
        public int MedicineQuantity { get; set; }
    }
}
