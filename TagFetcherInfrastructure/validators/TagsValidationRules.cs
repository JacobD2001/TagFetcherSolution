using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagFetcherInfrastructure.validators
{
    public static class TagsValidationRules
    {
        public static ValidationResult ValidateSortBy(string? sortBy, ValidationContext context)
        {
            var allowedSortByValues = new[] { "name", "share", "" };

            if (string.IsNullOrWhiteSpace(sortBy) || allowedSortByValues.Contains(sortBy.ToLower()))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"Invalid parameter for SortBy. Valid parameters are: {string.Join(", ", allowedSortByValues)}.");
            }
        }
    }
}
