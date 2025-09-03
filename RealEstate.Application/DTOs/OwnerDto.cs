namespace RealEstate.Application.DTOs
{
    public class OwnerDto
    {
        public int IdOwner { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Photo { get; set; }
        public DateTime Birthday { get; set; }
    }
}