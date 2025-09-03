using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Common;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Infrastructure.Repositories
{
    public class PropertyRepository : RepositoryBase<Property>, IPropertyRepository
    {
        public PropertyRepository(AppDbContext context) : base(context) { }

        public async Task<Property?> GetPropertyDetailsAsync(int propertyId)
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .Include(p => p.Traces)
                .FirstOrDefaultAsync(p => p.IdProperty == propertyId);
        }

        public async Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(int ownerId)
        {
            return await _context.Properties
                .Where(p => p.IdOwner == ownerId)
                .Include(p => p.Images)
                .Include(p => p.Traces)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PagedResult<Property>> ListPagedAsync(PropertyFilter filter)
        {
            var query = _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .Include(p => p.Traces)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(p => p.Name.Contains(filter.Name));
            if (!string.IsNullOrEmpty(filter.Address))
                query = query.Where(p => p.Address.Contains(filter.Address));
            if (!string.IsNullOrEmpty(filter.CodeInternal))
                query = query.Where(p => p.CodeInternal.Contains(filter.CodeInternal));
            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            if (filter.MinYear.HasValue)
                query = query.Where(p => p.Year >= filter.MinYear.Value);
            if (filter.MaxYear.HasValue)
                query = query.Where(p => p.Year <= filter.MaxYear.Value);
            if (filter.OwnerId.HasValue)
                query = query.Where(p => p.IdOwner == filter.OwnerId.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.IdProperty) // Default sort
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Property> { Items = items, TotalCount = totalCount, PageNumber = filter.PageNumber, PageSize = filter.PageSize };
        }

        public async Task AddImageAsync(PropertyImage image)
        {
            await _context.PropertyImages.AddAsync(image);
        }

        public async Task AddTraceAsync(PropertyTrace trace)
        {
            await _context.PropertyTraces.AddAsync(trace);
        }
    }
}