using Microsoft.EntityFrameworkCore;
using TagFetcherDomain.models;

namespace TagFetcherInfrastructure.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } 
        public virtual DbSet<Tag> Tags { get; set; }
    }
}
