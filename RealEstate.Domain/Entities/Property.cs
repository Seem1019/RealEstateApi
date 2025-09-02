namespace RealEstate.Domain.Entities
{
    public class Property
    {
        public int IdProperty { get; private set; } // PK.
        public string Name { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public string CodeInternal { get; private set; } = string.Empty; // Unique internal code.
        public int Year { get; private set; }
        public int IdOwner { get; private set; } // FK.
        public Owner? Owner { get; private set; } // Navigation.
        public ICollection<PropertyImage> Images { get; private set; } = new List<PropertyImage>();
        public ICollection<PropertyTrace> Traces { get; private set; } = new List<PropertyTrace>();

        private Property() { } // For EF Core.

        public static Property Create(string name, string address, decimal price, string codeInternal, int year, int idOwner)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty", nameof(name));
            if (price <= 0) throw new ArgumentException("Price must be positive", nameof(price));
            return new Property { Name = name, Address = address, Price = price, CodeInternal = codeInternal, Year = year, IdOwner = idOwner };
        }

        public void ChangePrice(decimal newPrice)
        {
            if (newPrice <= 0) throw new ArgumentException("Price must be positive", nameof(newPrice));
            Price = newPrice;
        }

        public void UpdateDetails(string name, string address, decimal price, string codeInternal, int year)
        {
            Name = name ?? Name;
            Address = address ?? Address;
            ChangePrice(price);
            CodeInternal = codeInternal ?? CodeInternal;
            Year = year;
        }

        public void SetOwner(Owner owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (Owner != null && Owner.IdOwner != owner.IdOwner) throw new InvalidOperationException("Cannot change owner");
            Owner = owner;
            IdOwner = owner.IdOwner;
        }

        public void AddImage(PropertyImage image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            Images.Add(image);
        }

        public void AddTrace(PropertyTrace trace)
        {
            if (trace == null) throw new ArgumentNullException(nameof(trace));
            Traces.Add(trace);
        }
    }
}