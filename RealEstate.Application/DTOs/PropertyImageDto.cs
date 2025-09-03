namespace RealEstate.Application.DTOs
{
    public class PropertyImageDto
    {
        public int IdPropertyImage { get; set; }
        public string File { get; set; } = string.Empty;
        public bool Enabled { get; set; }
    }
}