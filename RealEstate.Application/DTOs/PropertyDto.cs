namespace RealEstate.Application.DTOs
{
    public class PropertyDto
    {
        public int IdProperty { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CodeInternal { get; set; } = string.Empty;
        public int Year { get; set; }
        public int IdOwner { get; set; }
        public OwnerDto? Owner { get; set; }
        public List<PropertyImageDto> Images { get; set; } = new List<PropertyImageDto>();
        public List<PropertyTraceDto> Traces { get; set; } = new List<PropertyTraceDto>();
    }
}