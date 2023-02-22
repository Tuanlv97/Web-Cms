namespace Cms.Api.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ActivityLog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; }

        [Required]
        [StringLength(50)]
        public string EntityName { get; set; }

        [Required]
        [StringLength(50)]
        public string EntityId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? LastModifiedDate { get; set; }

        [StringLength(50)]
        public string UserId { get; set; }

        [StringLength(500)]
        public string Content { get; set; }
    }
}
