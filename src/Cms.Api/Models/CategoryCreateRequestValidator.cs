using Cms.ViewModels;
using FluentValidation;

namespace Cms.Api.Models
{

    public class CategoryCreateRequestValidator : AbstractValidator<CategoryCreateRequest>
    {
        public CategoryCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(string.Format(Messages.Required, "Tên"));

            RuleFor(x => x.SeoAlias).NotEmpty().WithMessage(string.Format(Messages.Required, "Seo alias"));
        }
    }
}