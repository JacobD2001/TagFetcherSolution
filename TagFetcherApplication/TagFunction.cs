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
using TagFetcherInfrastructure.interfaces;
using TagFetcherInfrastructure.services;

namespace TagFetcherApplication
{
    internal class TagFunction
    {
        private readonly IStackOverflowService _stackOverflowService;
        private readonly ITagService _tagService;

        public TagFunction(IStackOverflowService stackOverflowService, ITagService tagService)
        {
            _stackOverflowService = stackOverflowService;
            _tagService = tagService;
        }

        [Function("FetchAndSaveTags")]
        public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
        {
            try
            {
                var existingTags = await _tagService.GetAllTagsFromDbAsync();
                if (existingTags != null && existingTags.Any())
                {
                    _tagService.DeleteAllTagsAsync();
                }
                // TO DO : Loggs cause error for some reason 
                //log.LogInformation("Fetching tags from StackOverflow API...");
                var tags = await _stackOverflowService.FetchTagsAsync();

               // log.LogInformation($"{tags.Count} tags fetched successfully.");

                await _stackOverflowService.SaveTagsAsync(tags);
               // log.LogInformation($"{tags.Count} tags saved successfully.");

                await _stackOverflowService.CalculateShareAsync();

                return new OkResult();
            }
            catch (Exception)
            {
               // log.LogError($"Unexpected error occurred when fetching tags {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
