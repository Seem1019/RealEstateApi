using RealEstate.Application.DTOs;
using RealEstate.Domain.Common;

namespace RealEstate.Application.Services
{
    public interface IPropertyService
    {
        Task<PropertyDto> CreateAsync(CreatePropertyDto dto);
        Task AddImageAsync(AddImageDto dto);
        Task ChangePriceAsync(ChangePriceDto dto);
        Task<PropertyDto> UpdateAsync(int id, UpdatePropertyDto dto);
        Task<PagedResult<PropertyDto>> ListAsync(PropertyFilter filter);
        Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(int ownerId);
        Task<PropertyDetailsDto> GetPropertyDetailsAsync(int propertyId);
    }
}