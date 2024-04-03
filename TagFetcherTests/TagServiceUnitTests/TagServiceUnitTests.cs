using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagFetcherInfrastructure.data;
using TagFetcherInfrastructure.interfaces;
using TagFetcherInfrastructure.queryParamsModels;
using TagFetcherInfrastructure.services;
using Xunit;

namespace TagFetcherTests.TagServiceUnitTests
{
    public class TagServiceUnitTests
    {
        private readonly ITagService _service;
        private readonly AppDbContext _context;

        public TagServiceUnitTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);
            _service = new TagService(_context);

            TestDataSeeder.SeedDatabase(_context);
        }

        [Fact]
        public async Task GetAllTagsFromDbAsync_ReturnsAllTags()
        {
            // Arrange - check if there are two tags in the in-memory database
            var initialCount = await _context.Tags.CountAsync();
            Assert.Equal(2, initialCount);

            // Act - get all tags from the in-memory database
            var result = await _service.GetAllTagsFromDbAsync();

            // Assert - check if all tags are returned
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task DeleteAllTagsAsync_DeletesAllTags()
        {
            // Arrange
            var initialCount = await _context.Tags.CountAsync();
            Assert.Equal(2, initialCount);

            // Act
            await _service.DeleteAllTagsAsync();

            // Assert
            var countAfterDeletion = await _context.Tags.CountAsync();
            Assert.Equal(0, countAfterDeletion);
        }

        [Fact]
        public async Task GetTagsAsync_SortsAndPaginatesCorrectly()
        {
            // Arrange
            var queryParameters = new TagsQueryParameters
            {
                PageSize = 1,
                PageNumber = 2,
                SortBy = "name",
                SortOrder = "asc"
            };

            // Act
            var result = await _service.GetTagsAsync(queryParameters);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Tag 2", result.First().Name);
        }

    }
}
