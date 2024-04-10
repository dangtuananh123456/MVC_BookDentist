
using AutoMapper;
using DataModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using WebApplication.Congfig;
using DataModels.Config;
using DataModels.DbContexts;

namespace WebApplication
{
    public class Program
	{
		public static void Main(string[] args)
		{
			var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			builder.Services.AddDbContext<AppDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),
					x => x.UseDateOnlyTimeOnly());
			});

			builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
			{
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 2;
			})
			.AddEntityFrameworkStores<AppDbContext>();

			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/Account/Login";
				options.AccessDeniedPath = "/Account/AccessDenied";
			});

            builder.Services.AddSingleton<DapperContext>();

            builder.Services.AddScoped<CustomerRepository>();
			builder.Services.AddScoped<DentistRepository>();
			builder.Services.AddScoped<EmployeeRepository>();
			builder.Services.AddScoped<AppointmentScheduleRepository>();
			builder.Services.AddScoped<MedicineRepository>();
			builder.Services.AddScoped<MedicineInventoryRepository>();
			builder.Services.AddScoped<MedicalRecordRespository>();
			builder.Services.AddScoped<MedicineMedicalRecordRespository>();
			builder.Services.AddScoped<CreditRepository>();
			// Auto Mapper Configurations
			var mapperConfig = new MapperConfiguration(mc =>
			{
				mc.AddProfile(new ConfigAutoMapper());
			});

			IMapper mapper = mapperConfig.CreateMapper();
			builder.Services.AddSingleton(mapper);
			builder.Services.AddMvc();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}