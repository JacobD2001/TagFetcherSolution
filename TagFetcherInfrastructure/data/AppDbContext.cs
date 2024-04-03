using Microsoft.EntityFrameworkCore;
using TagFetcherDomain.models;

namespace TagFetcherInfrastructure.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } 
        public DbSet<Tag> Tags { get; set; }
    }
}
