namespace Cms.Api.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Function
    {
        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Url { get; set; }

        public int SortOrder { get; set; }

        [StringLength(50)]
        public string ParentId { get; set; }

        [StringLength(50)]
        public string Icon { get; set; }
    }
}
