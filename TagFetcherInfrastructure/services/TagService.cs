using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagFetcherDomain.models;
using TagFetcherInfrastructure.data;
using TagFetcherInfrastructure.dtoModels;
using TagFetcherInfrastructure.interfaces;
using TagFetcherInfrastructure.queryParamsModels;

namespace TagFetcherInfrastructure.services
{
    public class TagService : ITagService
    {
        private readonly AppDbContext _context;

        public TagService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetAllTagsFromDbAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<List<TagDto>> GetTagsAsync(TagsQueryParameters queryParameters)
        {
            var query = _context.Tags.AsQueryable();

            // sorting
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                var isAscending = string.IsNullOrEmpty(queryParameters.SortOrder) || queryParameters.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);

                switch (queryParameters.SortBy.ToLower())
                {
                    case "name":
                        query = isAscending ? query.OrderBy(t => t.Name) : query.OrderByDescending(t => t.Name);
                        break;
                    case "share":
                        query = isAscending ? query.OrderBy(t => t.Share) : query.OrderByDescending(t => t.Share);
                        break;
                    default:
                        // Default sorting by name if no valid sorting parameter is provided
                        query = query.OrderBy(t => t.Name);
                        break;
                }
            }

            // Pagination
            var tags = await query
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(tag => new TagDto { Name = tag.Name, Count = tag.Count, Share = tag.Share })
                .ToListAsync();

            return tags;
        }

    }
}
