namespace RealEstate.Application.DTOs
{
    public class AddImageDto
    {
        public int PropertyId { get; set; }
        public string File { get; set; } = string.Empty;
    }
}