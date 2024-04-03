using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TagFetcherDomain.models;
using TagFetcherInfrastructure.data;
using TagFetcherInfrastructure.interfaces;
using TagFetcherInfrastructure.responseModels;

//TO DO : Exception handling and return http codes - maybe in model validaiton or in azure func
namespace TagFetcherInfrastructure.services
{
    public class StackOverflowService : IStackOverflowService
    {
        private readonly AppDbContext _context;
        public StackOverflowService(AppDbContext context)
        {
            _context = context;
        }

        // Get tags from StackOverflow API 
        public async Task<List<Tag>> FetchTagsAsync()
        {
            var tags = new List<Tag>();
            var client = new RestClient("https://api.stackexchange.com/2.3/");
            int pagesToFetch = 10;

            for (int page = 1; page <= pagesToFetch; page++)
            {
                var request = new RestRequest($"tags?pagesize=100&site=stackoverflow&page={page}", Method.Get);
                var response = await client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    var result = JsonConvert.DeserializeObject<StackOverflowTagsResponse>(response.Content);
                    tags.AddRange(result.Items);
                }
                else
                {
                    throw new Exception("Failed to fetch tags from StackOverflow API");
                }
            }

            return tags;
        }

        // Save tags to database
        public async Task SaveTagsAsync(List<Tag> tags)
        {
            var tagNames = tags.Select(t => t.Name);
            var existingTags = await _context.Tags.Where(t => tagNames.Contains(t.Name)).ToDictionaryAsync(t => t.Name);

            foreach (var tag in tags)
            {
                if (existingTags.TryGetValue(tag.Name, out var existingTag))
                {
                    existingTag.Count = tag.Count;
                }
                else
                {
                    await _context.Tags.AddAsync(tag);
                }
            }

            await _context.SaveChangesAsync();
        }

        // Calculate share for each tag
        public async Task CalculateShareAsync()
        {
            var totalTags = await _context.Tags.SumAsync(t => t.Count);
            var tags = await _context.Tags.ToListAsync();

            foreach (var tag in tags)
            {
                tag.Share = Math.Round((double)tag.Count / totalTags * 100, 2);
            }

            await _context.SaveChangesAsync();
        }

    }
}
