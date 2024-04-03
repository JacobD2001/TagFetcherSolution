using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagFetcherDomain.models;
using TagFetcherInfrastructure.data;

namespace TagFetcherInfrastructure.interfaces
{
    public interface IStackOverflowService
    {
        Task<List<Tag>> FetchTagsAsync();
        Task SaveTagsAsync(List<Tag> tags);
        Task CalculateShareAsync();
    }
}
