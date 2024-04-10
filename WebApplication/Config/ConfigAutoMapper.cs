using AutoMapper;
using DataModels;
using WebApplication.Models;

namespace WebApplication.Congfig
{
    public class ConfigAutoMapper : Profile
    {
        public ConfigAutoMapper()
        {
            CreateMap<DateTime, DateOnly>().ConvertUsing<DateTimeToDateOnlyConverter>();
            CreateMap<DateOnly, DateTime>().ConvertUsing<DateOnlyToDateTimeConverter>();
            CreateMap<Medicine, CreateMedicineModel>().ReverseMap();
            CreateMap<MedicineInventory, WebApplication.Models.MedicineInventoryModel>().ReverseMap();
            CreateMap<MedicalRecord, EditMedicalRecordModel>().ReverseMap();
            CreateMap<MedicineMedicalRecordModel, Medicine_MedicalRecord>().ReverseMap();
            CreateMap<Customer, CustomerModel>().ReverseMap();
        }
    }

    public class DateTimeToDateOnlyConverter : ITypeConverter<DateTime, DateOnly>
    {
        public DateOnly Convert(DateTime source, DateOnly destination, ResolutionContext context)
        {
            return new DateOnly(source.Year, source.Month, source.Day);
        }
    }
    public class DateOnlyToDateTimeConverter : ITypeConverter<DateOnly, DateTime>
    {
        public DateTime Convert(DateOnly source, DateTime destination, ResolutionContext context)
        {
            return new DateTime(source.Year, source.Month, source.Day);
        }
    }

}
