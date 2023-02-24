using FluentValidation.Attributes;

namespace Cms.Api.Models
{
    [Validator(typeof(CategoryCreateRequestValidator))]
    public class CategoryCreateRequest
    {
        public string Name { get; set; }

        public string SeoAlias { get; set; }

        public string SeoDescription { get; set; }

        public int SortOrder { get; set; }

        public int? ParentId { get; set; }
    }
}