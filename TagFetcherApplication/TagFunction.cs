using Microsoft.AspNetCore.Http;
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
            var tags = await _stackOverflowService.FetchTagsAsync();
            await _stackOverflowService.SaveTagsAsync(tags, _dbContext);
            return new OkObjectResult($"{tags.Count}Tags saved successfully");
        }


    }
}
