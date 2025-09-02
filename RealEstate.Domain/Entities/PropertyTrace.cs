namespace RealEstate.Domain.Entities
{
    public class PropertyTrace
    {
        public int IdPropertyTrace { get; private set; }
        public int IdProperty { get; private set; }
        public Property? Property { get; private set; }
        public DateTime DateSale { get; private set; }
        public string Name { get; private set; } = string.Empty; // e.g., Transaction name.
        public decimal Value { get; private set; } // Sale value.
        public decimal Tax { get; private set; }

        private PropertyTrace() { } // For EF Core.

        public static PropertyTrace Create(int idProperty, DateTime dateSale, string name, decimal value, decimal tax)
        {
            if (value <= 0) throw new ArgumentException("Value must be positive", nameof(value));
            return new PropertyTrace { IdProperty = idProperty, DateSale = dateSale, Name = name, Value = value, Tax = tax };
        }
    }
}