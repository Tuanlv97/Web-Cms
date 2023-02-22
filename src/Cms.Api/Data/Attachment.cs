namespace Cms.Api.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Attachment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string FileName { get; set; }

        [Required]
        [StringLength(200)]
        public string FilePath { get; set; }

        [Required]
        [StringLength(4)]
        public string FileType { get; set; }

        public long FileSize { get; set; }

        public int KnowledgeBaseId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? LastModifiedDate { get; set; }
    }
}
