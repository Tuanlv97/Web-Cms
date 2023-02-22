namespace Cms.Api.Models
{
    public class CommentCreateRequest
    {
        public string Content { get; set; }

        public int KnowledgeBaseId { get; set; }

        public int? ReplyId { get; set; }

        public string CaptchaCode { get; set; }
    }
}