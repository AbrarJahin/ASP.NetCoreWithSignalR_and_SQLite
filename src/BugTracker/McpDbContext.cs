using BugTracker.DbModels.Try;
using BugTracker.DbModels.Mcp;
using Microsoft.EntityFrameworkCore;

namespace BugTracker
{
    public class McpDbContext : DbContext
    {
        public McpDbContext(DbContextOptions<McpDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            //context.Database.Migrate();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Bulletin> Mcp { get; set; }
    }
}
