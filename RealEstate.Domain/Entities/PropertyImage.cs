namespace RealEstate.Domain.Entities
{
    public class PropertyImage
    {
        public int IdPropertyImage { get; private set; }
        public int IdProperty { get; private set; }
        public Property? Property { get; private set; }
        public string File { get; private set; } = string.Empty; // Path or base64-encoded image data.
        public bool Enabled { get; private set; } = true;

        private PropertyImage() { } // For EF Core.

        public static PropertyImage Create(int idProperty, string file)
        {
            if (string.IsNullOrWhiteSpace(file)) throw new ArgumentException("File cannot be empty", nameof(file));
            return new PropertyImage { IdProperty = idProperty, File = file };
        }

        public void Disable()
        {
            Enabled = false;
        }
    }
}