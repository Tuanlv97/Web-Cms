using System;

namespace Cms.Api.Models
{
    public class AttachmentVm
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }

        public long FileSize { get; set; }

        public int KnowledgeBaseId { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
