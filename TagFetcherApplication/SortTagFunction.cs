using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagFetcherInfrastructure.interfaces;
using TagFetcherInfrastructure.queryParamsModels;

namespace TagFetcherApplication
{
    public class SortTagFunction
    {
        private readonly ITagService _tagService;
        private readonly IStackOverflowService _stackOverflowService;

        public SortTagFunction(ITagService tagService, IStackOverflowService stackOverflowService)
        {
            _tagService = tagService;
            _stackOverflowService = stackOverflowService;
        }

        [Function("GetAndSortTags")]
        public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
        {
            try
            {
                //if there are no tags in database fetch from stackoverflow api
                var exisitingTags = await _tagService.GetAllTagsFromDbAsync();
                if(exisitingTags == null || !exisitingTags.Any())
                {
                    var fetchedTags = await _stackOverflowService.FetchTagsAsync();
                    await _stackOverflowService.SaveTagsAsync(fetchedTags);
                    await _stackOverflowService.CalculateShareAsync();
                }

                var queryParameters = new TagsQueryParameters
                {
                    PageSize = string.IsNullOrEmpty(req.Query["pageSize"]) ? 10 : int.Parse(req.Query["pageSize"]),
                    PageNumber = string.IsNullOrEmpty(req.Query["pageNumber"]) ? 1 : int.Parse(req.Query["pageNumber"]),
                    SortBy = req.Query["sortBy"],
                    SortOrder = req.Query["sortOrder"]
                };


                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(queryParameters, serviceProvider: null, items: null);
                bool isValid = Validator.TryValidateObject(queryParameters, validationContext, validationResults, true);

                if (!isValid)
                {
                    return new BadRequestObjectResult(validationResults.Select(x => x.ErrorMessage));
                }

                var tags = await _tagService.GetTagsAsync(queryParameters);
                return new OkObjectResult(tags);

            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error occurred when fetching tags {ex.Message}");
            }

        }

    }
}
