using Microsoft.EntityFrameworkCore;
using TOKENDEMO.Models;

namespace TOKENDEMO.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _dbContext;
        public UserRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
             _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _dbContext.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            var userExisted = await GetByIdAsync(user.Id);
            _dbContext.Entry(userExisted).CurrentValues.SetValues(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
    }
}
