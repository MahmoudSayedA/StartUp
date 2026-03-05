using System.ComponentModel.DataAnnotations;

namespace Application.Common.Abstractions.Collections
{
    public abstract class HasTableView : HasPagination
    {
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public List<FilterDescriptor>? Filters { get; set; }

        public HasTableView()
        {
        }
    }

    public class FilterDescriptor
    {
        public string Key { get; set; } = string.Empty;

        [AllowedValues([null, "==", "!=", ">", ">=", "<", "<=", "contains", "startsWith", "in", "between"])]
        public string? Operator { get; set; } = "=="; // ==, !=, >, >=, <, <=, contains, startsWith, in
        public string? Value { get; set; }
        public string? ValueTo { get; set; } // for "between"
    }
}
