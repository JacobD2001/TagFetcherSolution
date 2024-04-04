using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using TagFetcherDomain.models;
using TagFetcherInfrastructure.data;
using TagFetcherInfrastructure.interfaces;
using TagFetcherInfrastructure.responseModels;
using TagFetcherInfrastructure.validators;

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
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.TooManyRequests:
                            throw new StackOverflowApiException("Rate limit exceeded. Please try again later.", response.StatusCode, "RateLimitExceeded");
                        case System.Net.HttpStatusCode.NotFound:
                            throw new StackOverflowApiException("Resource not found.", response.StatusCode, "NotFound");
                        case System.Net.HttpStatusCode.BadRequest:
                            throw new StackOverflowApiException("Bad request.", response.StatusCode, "BadParameter");
                        case System.Net.HttpStatusCode.ProxyAuthenticationRequired:
                            throw new StackOverflowApiException("Write failed.", response.StatusCode, "WriteFailed");
                        case System.Net.HttpStatusCode.Conflict:
                            throw new StackOverflowApiException("Duplicate request.", response.StatusCode, "DuplicateRequest");
                        case System.Net.HttpStatusCode.InternalServerError:
                            throw new StackOverflowApiException("Internal error.", response.StatusCode, "InternalError");
                        case System.Net.HttpStatusCode.BadGateway:
                            throw new StackOverflowApiException("Throttle violation.", response.StatusCode, "ThrottleViolation");
                        case System.Net.HttpStatusCode.ServiceUnavailable:
                            throw new StackOverflowApiException("Temporarily unavailable.", response.StatusCode, "TemporarilyUnavailable");
                        default:
                            throw new StackOverflowApiException("Unexpected error occurred.", response.StatusCode, "UnexpectedError");
                    }
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
