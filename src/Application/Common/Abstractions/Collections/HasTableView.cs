using FluentValidation.Results;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Common.Abstractions.Collections
{
    public abstract class HasTableView : HasPagination
    {
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public Dictionary<string, string>? Filters { get; set; }

        public HasTableView()
        {
        }
        public bool ValidateFiltersAndSorting(List<string>? allowedFilters, List<string>? allowedSorting)
        {
            ValidateFilters(allowedFilters);
            ValidateSorting(allowedSorting);
            return true;
        }
        private void ValidateSorting(List<string>? allowed)
        {
            if (allowed == null || allowed.Count == 0)
                return;

            if (!string.IsNullOrEmpty(SortBy) && !allowed.Contains(SortBy))
            {
                ICollection<ValidationFailure> validationFailures = [new ValidationFailure(nameof(SortBy), $"Invalid property name ({SortBy}). Only values [{string.Join(", ", allowed)}] are allowed.")];
                throw new ValidationException(validationFailures);
            }
        }

        private void ValidateFilters(List<string>? allowed)
        {
            if (allowed == null || allowed.Count == 0)
                return;

            if (Filters != null && Filters.Count != 0)
            {
                foreach (var filter in Filters)
                {
                    if (!allowed.Contains(filter.Key))
                    {
                        List<ValidationFailure> validationFailures = [new ValidationFailure(filter.Key, $"Invalid property name ({filter.Key}). Only values [{string.Join(", ", allowed)}] are allowed.")];
                        throw new ValidationException(validationFailures);
                    }
                }
            }
        }
    }
}
