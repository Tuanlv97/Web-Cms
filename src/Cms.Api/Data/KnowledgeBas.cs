namespace Cms.Api.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KnowledgeBases")]
    public partial class KnowledgeBas
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string SeoAlias { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Environment { get; set; }

        [StringLength(500)]
        public string Problem { get; set; }

        public string StepToReproduce { get; set; }

        [StringLength(500)]
        public string ErrorMessage { get; set; }

        [StringLength(500)]
        public string Workaround { get; set; }

        public string Note { get; set; }

        [Required]
        [StringLength(50)]
        public string OwnerUserId { get; set; }

        public string Labels { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? LastModifiedDate { get; set; }

        public int? NumberOfComments { get; set; }

        public int? NumberOfVotes { get; set; }

        public int? NumberOfReports { get; set; }

        public int? ViewCount { get; set; }
    }
}
