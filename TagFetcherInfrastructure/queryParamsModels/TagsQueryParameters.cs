using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagFetcherInfrastructure.validators;

namespace TagFetcherInfrastructure.queryParamsModels
{
    public class TagsQueryParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;

        [CustomValidation(typeof(TagsValidationRules), nameof(TagsValidationRules.ValidateSortBy))]
        public string? SortBy { get; set; } = "Name";
        public string? SortOrder { get; set; } = "asc";
    }
}
