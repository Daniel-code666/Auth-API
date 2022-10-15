using Auth_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Users>? User { get; set; }

    }
}
