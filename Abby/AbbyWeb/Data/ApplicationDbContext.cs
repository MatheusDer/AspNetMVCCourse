using AbbyWeb.Model;
using Microsoft.EntityFrameworkCore;

namespace AbbyWeb.Data
{
    public class ApplicationDbContext : DbContext  //add-migration <migrationName> database-update
    {
        public DbSet<Category> Categories { get; set; } //Name of the table in database

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
    }
}
