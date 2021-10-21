using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Ms1.Api.Store
{
    public class InsideTestDbContext : DbContext
    {
        public InsideTestDbContext(DbContextOptions options) :base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Message> Messages { get; set; }
    }
}
