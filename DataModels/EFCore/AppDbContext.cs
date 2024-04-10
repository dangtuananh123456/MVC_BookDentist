using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bogus;

namespace DataModels.DbContexts
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //key for AppointmentSchedule
            builder.Entity<AppointmentSchedule>()
                .HasKey(x => new { x.DentistId, x.StartTime });
            // n-n relationship
            builder.Entity<Medicine_MedicalRecord>()
                .HasKey(x => new { x.MedicineId, x.MedicalRecordId, x.SequenceNumber });
            builder.Entity<Medicine_MedicalRecord>()
                .HasOne(m => m.Medicine)
                .WithMany(mmr => mmr.Medicine_MedicalRecords)
                .HasForeignKey(mmr => mmr.MedicineId);
            builder.Entity<Medicine_MedicalRecord>()
                .HasOne(m => m.MedicalRecord)
                .WithMany(mmr => mmr.Medicine_MedicalRecords)
                .HasForeignKey(mmr => new { mmr.MedicalRecordId, mmr.SequenceNumber });

            //
            builder.Entity<MedicalRecord>()
                .HasKey(x => new { x.Id, x.SequenceNumber });

            builder.Entity<MedicalRecord>()
            .HasOne(mr => mr.Customer)
            .WithMany()
            .HasForeignKey(mr => mr.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<MedicalRecord>()
                .HasOne(mr => mr.CreatedByDentist)
                .WithMany()
                .HasForeignKey(mr => mr.CreatedByDentistId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<MedicalRecord>()
                .HasOne(mr => mr.ExamDentist)
                .WithMany()
                .HasForeignKey(mr => mr.ExamDentistId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Dentist>()
                .HasOne(d => d.Account)
                .WithMany()
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.NoAction);




            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.HasKey(r => new { r.UserId, r.RoleId });
            });


            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
        }


        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Dentist> Dentists { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AppointmentSchedule> AppointmentSchedules { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineInventory> MedicineInventories { get; set; }
        public DbSet<Medicine_MedicalRecord> Medicine_MedicalRecords { get; set; }
        public DbSet<Credit> Credits { set; get; }
    }
}