namespace DataModels
{
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Prescription { get; set; }
        public decimal Price { get; set; }
        public ICollection<Medicine_MedicalRecord> Medicine_MedicalRecords { get; set; }
    }
}
