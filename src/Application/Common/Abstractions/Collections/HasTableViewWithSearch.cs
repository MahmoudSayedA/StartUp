namespace Application.Common.Abstractions.Collections
{
    public abstract class HasTableViewWithSearch : HasTableView
    {
        public string? Search { get; set; }
        public List<string>? SearchColumns { get; set; }
    }
}

