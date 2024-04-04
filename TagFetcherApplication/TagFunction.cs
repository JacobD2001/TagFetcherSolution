using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TagFetcherInfrastructure.data;
using TagFetcherInfrastructure.interfaces;
using TagFetcherInfrastructure.services;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using TagFetcherInfrastructure.validators;


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
        [OpenApiOperation(operationId: "fetchAndSaveTags", tags: new[] { "tag" }, Summary = "Fetch and save tags", Description = "This function forces re-fetching tags from StackOverflow API and saves them into the database.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(void), Description = "The OK response indicating that tags were successfully fetched and saved.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "Unexpected error occurred")]
        public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
        {
            try
            {
                var existingTags = await _tagService.GetAllTagsFromDbAsync();
                if (existingTags != null && existingTags.Any())
                {
                    await _tagService.DeleteAllTagsAsync();
                }

                var tags = await _stackOverflowService.FetchTagsAsync();
                await _stackOverflowService.SaveTagsAsync(tags);
                await _stackOverflowService.CalculateShareAsync();

                return new OkResult();
            }
            catch (StackOverflowApiException ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = (int)ex.StatusCode };
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
