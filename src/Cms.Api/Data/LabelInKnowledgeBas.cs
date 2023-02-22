namespace Cms.Api.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LabelInKnowledgeBases")]
    public partial class LabelInKnowledgeBas
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KnowledgeBaseId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string LabelId { get; set; }
    }
}
