using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TagFetcherInfrastructure.dtoModels;
using TagFetcherInfrastructure.interfaces;
using TagFetcherInfrastructure.queryParamsModels;
using TagFetcherInfrastructure.validators;

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
        [OpenApiOperation(operationId: "getAndSortTags", tags: new[] { "tag" }, Summary = "Get and sort tags", Description = "This function retrieves tags and sorts them based on query parameters. If no parameters are specified the default sorting takes place.")]
        [OpenApiParameter(name: "pageSize", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "Number of tags per page. Accepts values within range 1-100. Default value is 10.")]
        [OpenApiParameter(name: "pageNumber", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "Page number. The value must be an integer greater than 0. Default value is 1.")]
        [OpenApiParameter(name: "sortBy", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Sort by field. Accepts the following values: 'name', 'share', or an empty value ''. Default value is 'name'.")]
        [OpenApiParameter(name: "sortOrder", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Sort order: asc or desc. Asc - for ascending order. Descending order applies by default or with any other value than 'asc' specified.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<TagDto>), Description = "The OK response containing sorted tags.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Bad request when query parameters are invalid.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "Unexpected error occurred")]
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
            catch (StackOverflowApiException ex)
            {
                // log.LogError($"API Error: {ex.Message}");
                return new ObjectResult(ex.Message) { StatusCode = (int)ex.StatusCode };
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error occurred when fetching tags {ex.Message}");
            }

        }

    }
}
