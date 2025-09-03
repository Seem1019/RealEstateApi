using FluentValidation;
using RealEstate.Application.DTOs;

namespace RealEstate.Application.Validators
{
    public class CreatePropertyDtoValidator : AbstractValidator<CreatePropertyDto>
    {
        public CreatePropertyDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.CodeInternal).NotEmpty();
            RuleFor(x => x.Year).GreaterThan(1900); 
            RuleFor(x => x.IdOwner).GreaterThan(0);
        }
    }
}