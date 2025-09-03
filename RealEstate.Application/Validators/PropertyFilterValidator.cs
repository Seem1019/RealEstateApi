using FluentValidation;
using RealEstate.Domain.Common;

namespace RealEstate.Application.Validators
{
    public class PropertyFilterValidator : AbstractValidator<PropertyFilter>
    {
        public PropertyFilterValidator()
        {
            RuleFor(f => f.PageNumber).GreaterThan(0);
            RuleFor(f => f.PageSize).InclusiveBetween(1, 100);
            RuleFor(f => f.MinPrice).LessThanOrEqualTo(f => f.MaxPrice).When(f => f.MinPrice.HasValue && f.MaxPrice.HasValue);
            RuleFor(f => f.MinYear).LessThanOrEqualTo(f => f.MaxYear).When(f => f.MinYear.HasValue && f.MaxYear.HasValue);
            RuleFor(f => f.MinPrice).GreaterThanOrEqualTo(0).When(f => f.MinPrice.HasValue);
        }
    }
}