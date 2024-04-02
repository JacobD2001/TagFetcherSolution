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
using TagFetcherInfrastructure.responseModels;

//TO DO : Exception handling and return http codes - maybe in model validaiton or in azure func
// https://www.youtube.com/watch?v=gMwAhKddHYQ - handle excpetions
namespace TagFetcherInfrastructure.services
{
    public class StackOverflowService
    {

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

        //Save tags to database
        public async Task SaveTagsAsync(List<Tag> tags, AppDbContext dbContext)
        {
            foreach (var tag in tags)
            {
                //var existingTag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);
                //if (existingTag == null)
                //{
                    await dbContext.Tags.AddAsync(tag);
                //}
                //else
                //{
                  //  existingTag.Count = tag.Count;
                    //dbContext.Tags.Update(existingTag);
                //}
            }
            await dbContext.SaveChangesAsync();
        }


    }
}
