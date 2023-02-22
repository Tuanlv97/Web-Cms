namespace Cms.Api.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public int KnowledgeBaseId { get; set; }

        [StringLength(50)]
        public string OwnerUserId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? LastModifiedDate { get; set; }

        public int? ReplyId { get; set; }
    }
}
