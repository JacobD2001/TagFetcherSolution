using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagFetcherDomain.models;
using TagFetcherInfrastructure.dtoModels;
using TagFetcherInfrastructure.queryParamsModels;

namespace TagFetcherInfrastructure.interfaces
{
    public interface ITagService
    {
        Task<List<TagDto>> GetTagsAsync(TagsQueryParameters queryParameters);
        Task<List<Tag>> GetAllTagsFromDbAsync();
        Task DeleteAllTagsAsync();
    }
}
