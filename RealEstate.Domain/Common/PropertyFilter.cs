namespace RealEstate.Domain.Common
{
    public class PropertyFilter
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? CodeInternal { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public int? OwnerId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}