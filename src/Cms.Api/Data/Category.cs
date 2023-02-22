namespace Cms.Api.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string SeoAlias { get; set; }

        [StringLength(500)]
        public string SeoDescription { get; set; }

        public int SortOrder { get; set; }

        public int? ParentId { get; set; }

        public int? NumberOfTickets { get; set; }
    }
}
