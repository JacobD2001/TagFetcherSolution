using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        public SortTagFunction(ITagService tagService)
        {
            _tagService = tagService;
        }

        [Function("GetTags")]
        public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
        {
            try
            {
                var queryParameters = new TagsQueryParameters
                {
                    PageNumber = int.Parse(req.Query["pageNumber"]),
                    PageSize = int.Parse(req.Query["pageSize"]),
                    SortBy = req.Query["sortBy"],
                    SortOrder = req.Query["sortOrder"]
                };

                var tags = await _tagService.GetTagsAsync(queryParameters);

                return new OkObjectResult(tags);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }

    }
}
