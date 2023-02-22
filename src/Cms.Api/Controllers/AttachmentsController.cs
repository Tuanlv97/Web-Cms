using Cms.Api.Data;
using Cms.Api.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Cms.Api.Controllers
{
    [RoutePrefix("api/attachments")]
    public class AttachmentsController : ApiController
    {
        private readonly CmsDbContext _context = new CmsDbContext();
        #region Attachments
        [Route("{knowledgeBaseId}/attachments")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAttachment(int knowledgeBaseId)
        {
            var query = await _context.Attachments
                .Where(x => x.KnowledgeBaseId == knowledgeBaseId)
                .Select(c => new AttachmentVm()
                {
                    Id = c.Id,
                    LastModifiedDate = c.LastModifiedDate,
                    CreateDate = c.CreateDate,
                    FileName = c.FileName,
                    FilePath = c.FilePath,
                    FileSize = c.FileSize,
                    FileType = c.FileType,
                    KnowledgeBaseId = c.KnowledgeBaseId
                }).ToListAsync();

            return Ok(query);
        }

        [Route("{knowledgeBaseId}/attachments/{attachmentId}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAttachment(int attachmentId)
        {
            var attachment = await _context.Attachments.FindAsync(attachmentId);
            if (attachment == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Cannot found attachment with id {attachmentId}"));

            _context.Attachments.Remove(attachment);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Delete attachment failed"));
        }
        #endregion
    }
}
