using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagFetcherInfrastructure.data;
using TagFetcherInfrastructure.services;
using Xunit;

namespace TagFetcherTests.StackOverflowServiceIntegrationTests
{
    public class StackOverflowServiceIntegrationTests
    {

        private readonly AppDbContext _context; 
        private readonly StackOverflowService _service;

        public StackOverflowServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new StackOverflowService(_context);
        }

        [Fact]
        public async Task FullIntegrationTest()
        {
            // Act
            var tags = await _service.FetchTagsAsync();

            await _service.SaveTagsAsync(tags);

            await _service.CalculateShareAsync();

            // Assert
            var savedTags = await _context.Tags.ToListAsync();
            Assert.NotEmpty(savedTags);

            var totalShare = savedTags.Sum(tag => tag.Share);
            Assert.Equal(100, Math.Round(totalShare));
        }
    }
}
