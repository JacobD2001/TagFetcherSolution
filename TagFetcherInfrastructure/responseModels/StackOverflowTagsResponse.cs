using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagFetcherDomain.models;

namespace TagFetcherInfrastructure.responseModels
{
    public class StackOverflowTagsResponse
    {
        [JsonProperty("items")]
        public List<Tag>? Items { get; set; }
    }
}
