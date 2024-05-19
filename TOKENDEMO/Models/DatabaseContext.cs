using Microsoft.EntityFrameworkCore;

namespace TOKENDEMO.Models
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext(DbContextOptions options):base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
