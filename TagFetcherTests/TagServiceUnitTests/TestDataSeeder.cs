using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagFetcherDomain.models;
using TagFetcherInfrastructure.data;

namespace TagFetcherTests.TagServiceUnitTests
{
    public static class TestDataSeeder
    {
        public static void SeedDatabase(AppDbContext context)
        {
            context.Tags.RemoveRange(context.Tags);
            context.SaveChanges();

            var tags = new List<Tag>
            {
            new Tag { Name = "Test Tag 1", Count = 1 },
            new Tag { Name = "Test Tag 2", Count = 2 }
            };

            context.Tags.AddRange(tags);
            context.SaveChanges();
        }
    }
}
