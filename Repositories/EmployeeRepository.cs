using DataModels;
using DataModels.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Repositories
{
    public class EmployeeRepository
    {
        private readonly AppDbContext dbContext;

        public EmployeeRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddEmployeeAsync(Employee employee)
        {
            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();
        }
        public async Task UpdateEmployeeAsync(Employee employee)
        {
            dbContext.Employees.Update(employee);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(Employee employee)
        {
            dbContext.Employees.Remove(employee);
            await dbContext.SaveChangesAsync();
        }
        public async Task<Employee> GetEmployeeByAccountAsync(AppUser user)
		{
			return await dbContext.Employees.Where(x => x.AccountId == user.Id).SingleOrDefaultAsync();
		}
	}
}
