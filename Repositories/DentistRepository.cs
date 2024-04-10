using DataModels;
using DataModels.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class DentistRepository
    {
        private readonly AppDbContext dbContext;

        public DentistRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Dentist>> GetAllAsync()
        {
            return await dbContext.Dentists.ToListAsync();
        }
        public async Task AddDentistAsync(Dentist dentist)
        {
            await dbContext.Dentists.AddAsync(dentist);
            await dbContext.SaveChangesAsync();
        }
        public async Task UpdateDentistAsync(Dentist dentist)
        {
            dbContext.Dentists.Update(dentist);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteDentistAsync(Dentist dentist)
        {
            dbContext.Dentists.Remove(dentist);
            await dbContext.SaveChangesAsync();
        }
        public async Task<Dentist> GetDentistByAccountAsync(AppUser user)
		{
			return await dbContext.Dentists.Where(x => x.AccountId == user.Id).SingleOrDefaultAsync();
		}
        public async Task<Dentist> GetDentistByIdAsync(string id)
        {
            return await dbContext.Dentists.FindAsync(id);
        }
    }
}
