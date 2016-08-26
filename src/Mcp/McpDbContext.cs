using McpSmyrilLine.DbModels.Mcp;
using Microsoft.EntityFrameworkCore;

namespace McpSmyrilLine
{
    public class McpDbContext : DbContext
    {
        public McpDbContext(DbContextOptions<McpDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            //context.Database.Migrate();
        }

        //List of Models
            //MCP - Admin
            public DbSet<Image>         Image           { get; set; }
            public DbSet<Description>   Description     { get; set; }
            public DbSet<Bulletin>      Bulletin        { get; set; }
            public DbSet<BulletinTime>  BulletinTime    { get; set; }
    }
}
