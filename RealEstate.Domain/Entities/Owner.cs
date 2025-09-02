namespace RealEstate.Domain.Entities
{
    public class Owner
    {
        public int IdOwner { get; private set; } // PK, auto-generated.
        public string Name { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public string? Photo { get; private set; } // Optional URL or base64 for photo.
        public DateTime Birthday { get; private set; }
        // Navigation
        public ICollection<Property> Properties { get; private set; } = new List<Property>();

        private Owner() { } // For EF Core.

        public static Owner Create(string name, string address, DateTime birthday, string? photo = null)
        {
\            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty", nameof(name));
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Address cannot be empty", nameof(address));
            return new Owner { Name = name, Address = address, Birthday = birthday, Photo = photo };
        }

        public void UpdateDetails(string name, string address, DateTime birthday, string? photo = null)
        {
            Name = name ?? Name;
            Address = address ?? Address;
            Birthday = birthday;
            Photo = photo ?? Photo;
        }

        public void AddProperty(Property property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (Properties.Any(p => p.IdProperty == property.IdProperty)) throw new InvalidOperationException("Property already owned");
            Properties.Add(property);
            property.SetOwner(this);
        }
    }
}