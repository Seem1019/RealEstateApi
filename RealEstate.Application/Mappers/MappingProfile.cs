using AutoMapper;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Property, PropertyDto>();
            CreateMap<Property, PropertyDetailsDto>();
            CreateMap<Owner, OwnerDto>();
            CreateMap<PropertyImage, PropertyImageDto>();
            CreateMap<PropertyTrace, PropertyTraceDto>();
            CreateMap<CreatePropertyDto, Property>();
            CreateMap<UpdatePropertyDto, Property>(); // Partial map
        }
    }
}