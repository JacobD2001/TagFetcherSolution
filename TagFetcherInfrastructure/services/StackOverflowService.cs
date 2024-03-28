using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TagFetcherDomain.models;
using TagFetcherInfrastructure.data;

//TO DO : To test (check code below and read about api response format now throws an error)
//TO DO : Exception handling and return http codes - maybe in model validaiton
namespace TagFetcherInfrastructure.services
{
    public class StackOverflowService
    {
        private readonly HttpClient? _httpClient;

        public StackOverflowService(HttpClient? httpClient)
        {
            _httpClient = httpClient;
        }

        // Get tags from StackOverflow API 
        public async Task<List<Tag>> FetchTagsAsync()
        {
            var tags = new List<Tag>();
            var response = await _httpClient.GetAsync("https://api.stackexchange.com/2.3/tags?order=desc&sort=popular&site=stackoverflow");
            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var items = json.RootElement.GetProperty("items");
                foreach(var item in items.EnumerateArray())
                {
                    var tag = new Tag
                    {
                        Name = item.GetProperty("name").GetString(),
                        Count = item.GetProperty("count").GetInt32()
                    };
                    tags.Add(tag);
                }
            }

            return tags;
        }

        //Save tags to database
        public async Task SaveTagsAsync(List<Tag> tags, AppDbContext dbContext)
        {
            foreach (var tag in tags)
            {
                var existingTag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);
                if (existingTag == null)
                {
                    await dbContext.Tags.AddAsync(tag);
                }
                else
                {
                    existingTag.Count = tag.Count;
                    dbContext.Tags.Update(existingTag);
                }
            }
            await dbContext.SaveChangesAsync();
        }





    }
}
