using RealEstate.Domain.Common;
using RealEstate.Domain.Entities;

namespace RealEstate.Domain.Interfaces
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<Property?> GetPropertyDetailsAsync(int propertyId);
        Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(int ownerId);
        Task<PagedResult<Property>> ListPagedAsync(PropertyFilter filter);
        Task AddImageAsync(PropertyImage image);
        Task AddTraceAsync(PropertyTrace trace);
    }
}