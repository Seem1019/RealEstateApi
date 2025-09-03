using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using RealEstate.Application.DTOs;
using RealEstate.Application.Validators;
using RealEstate.Domain.Common;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Exceptions;
using RealEstate.Domain.Interfaces;


namespace RealEstate.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<PropertyService> _logger;

        public PropertyService(IPropertyRepository repo, IMapper mapper, ILogger<PropertyService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PropertyDto> CreateAsync(CreatePropertyDto dto)
        {
            _logger.LogInformation("Creating property {PropertyName} for owner {OwnerId}", dto.Name, dto.IdOwner);
            var property = Property.Create(dto.Name, dto.Address, dto.Price, dto.CodeInternal, dto.Year, dto.IdOwner);
            await _repo.AddAsync(property);
            await _repo.SaveChangesAsync();
            return _mapper.Map<PropertyDto>(property);
        }

        public async Task AddImageAsync(AddImageDto dto)
        {
            _logger.LogInformation("Adding image to property {PropertyId}", dto.PropertyId);
            var property = await _repo.GetByIdAsync(dto.PropertyId);
            if (property == null) throw new NotFoundException($"Property {dto.PropertyId} not found");
            var image = PropertyImage.Create(dto.PropertyId, dto.File);
            property.AddImage(image);
            await _repo.AddImageAsync(image);
            await _repo.SaveChangesAsync();
        }

        public async Task ChangePriceAsync(ChangePriceDto dto)
        {
            _logger.LogInformation("Changing price for property {PropertyId} to {NewPrice}", dto.Id, dto.NewPrice);
            var property = await _repo.GetByIdAsync(dto.Id);
            if (property == null) throw new NotFoundException($"Property {dto.Id} not found");
            var oldPrice = property.Price;
            property.ChangePrice(dto.NewPrice);
            var trace = PropertyTrace.Create(dto.Id, DateTime.UtcNow, "Price Change", dto.NewPrice, 0); // Tax 0 for price change
            property.AddTrace(trace);
            await _repo.AddTraceAsync(trace);
            await _repo.UpdateAsync(property);
            await _repo.SaveChangesAsync();
        }

        public async Task<PropertyDto> UpdateAsync(int id, UpdatePropertyDto dto)
        {
            _logger.LogInformation("Updating property {PropertyId}", id);
            var property = await _repo.GetByIdAsync(id);
            if (property == null) throw new NotFoundException($"Property {id} not found");
            property.UpdateDetails(dto.Name, dto.Address, dto.Price, dto.CodeInternal, dto.Year);
            await _repo.UpdateAsync(property);
            await _repo.SaveChangesAsync();
            return _mapper.Map<PropertyDto>(property);
        }

        public async Task<PagedResult<PropertyDto>> ListAsync(PropertyFilter filter)
        {
            _logger.LogInformation("Listing properties with filter");
            var validator = new PropertyFilterValidator();
            ValidationResult validationResult = await validator.ValidateAsync(filter);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var pagedProperties = await _repo.ListPagedAsync(filter);
            var dtos = _mapper.Map<IEnumerable<PropertyDto>>(pagedProperties.Items);
            return new PagedResult<PropertyDto>
            {
                Items = dtos,
                TotalCount = pagedProperties.TotalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(int ownerId)
        {
            _logger.LogInformation("Getting properties for owner {OwnerId}", ownerId);
            var properties = await _repo.GetPropertiesByOwnerIdAsync(ownerId);
            return _mapper.Map<IEnumerable<PropertyDto>>(properties);
        }

        public async Task<PropertyDetailsDto> GetPropertyDetailsAsync(int propertyId)
        {
            _logger.LogInformation("Getting details for property {PropertyId}", propertyId);
            var property = await _repo.GetPropertyDetailsAsync(propertyId);
            if (property == null) throw new NotFoundException($"Property {propertyId} not found");
            return _mapper.Map<PropertyDetailsDto>(property);
        }
    }
}