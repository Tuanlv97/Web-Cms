namespace Cms.Api.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Permission
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string FunctionId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string RoleId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string CommandId { get; set; }
    }
}
