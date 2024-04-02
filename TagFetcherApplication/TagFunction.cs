using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagFetcherInfrastructure.data;
using TagFetcherInfrastructure.services;

namespace TagFetcherApplication
{
    internal class TagFunction
    {
        private readonly StackOverflowService _stackOverflowService;
        private readonly AppDbContext _dbContext;

        public TagFunction(StackOverflowService stackOverflowService, AppDbContext dbContext)
        {
            _stackOverflowService = stackOverflowService;
            _dbContext = dbContext;
        }

        [Function("FetchAndSaveTags")]
        public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
        {
            try
            {
                var tags = await _stackOverflowService.FetchTagsAsync();
                //log.LogInformation($"{tags.Count} tags fetched successfully.");

                //save tags
                await _stackOverflowService.SaveTagsAsync(tags, _dbContext);
                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError($"Unexpected error occurred when fetching tags {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
