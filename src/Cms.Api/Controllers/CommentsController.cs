using Cms.Api.Data;
using Cms.Api.Models;
using Cms.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Cms.Api.Controllers
{
    [RoutePrefix("api/comments")]
    public class CommentsController : ApiController
    {
        private readonly CmsDbContext _context = new CmsDbContext();

        [Route("{knowledgeBaseId}/comments/filter")]
        [HttpGet]
        //[ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.VIEW)]
        [ResponseType(typeof(Pagination<CommentVm>))]
        public async Task<IHttpActionResult> GetCommentsPaging(int? knowledgeBaseId, string filter, int pageIndex, int pageSize)
        {
            var query = from c in _context.Comments
                        join u in _context.AspNetUsers
                            on c.OwnerUserId equals u.Id
                        select new { c, u };
            if (knowledgeBaseId.HasValue)
            {
                query = query.Where(x => x.c.KnowledgeBaseId == knowledgeBaseId.Value);
            }
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.c.Content.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.c.CreateDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CommentVm()
                {
                    Id = c.c.Id,
                    Content = c.c.Content,
                    CreateDate = c.c.CreateDate,
                    KnowledgeBaseId = c.c.KnowledgeBaseId,
                    LastModifiedDate = c.c.LastModifiedDate,
                    OwnerUserId = c.c.OwnerUserId,
                    OwnerName = c.u.FirstName + " " + c.u.LastName
                })
                .ToListAsync();

            var pagination = new Pagination<CommentVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [Route("{knowledgeBaseId}/comments/{commentId}")]
        [HttpGet]
        //[ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.VIEW)]
        public async Task<IHttpActionResult> GetCommentDetail(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Cannot found comment with id: {commentId}"));
            var user = await _context.AspNetUsers.FindAsync(comment.OwnerUserId);
            var commentVm = new CommentVm()
            {
                Id = comment.Id,
                Content = comment.Content,
                CreateDate = comment.CreateDate,
                KnowledgeBaseId = comment.KnowledgeBaseId,
                LastModifiedDate = comment.LastModifiedDate,
                OwnerUserId = comment.OwnerUserId,
                OwnerName = user.FirstName + " " + user.LastName
            };

            return Ok(commentVm);
        }

        [Route("{knowledgeBaseId}/comments")]
        [HttpPost]
        //[ApiValidationFilter]
        public async Task<IHttpActionResult> PostComment(int knowledgeBaseId, [FromBody] CommentCreateRequest request)
        {
            var comment = new Comment()
            {
                Content = request.Content,
                KnowledgeBaseId = knowledgeBaseId,
                OwnerUserId = string.Empty,
                ReplyId = request.ReplyId
            };
            _context.Comments.Add(comment);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Cannot found knowledge base with id: {knowledgeBaseId}"));
            knowledgeBase.NumberOfComments = knowledgeBase.NumberOfComments.GetValueOrDefault(0) + 1;
            _context.KnowledgeBases.Attach(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                //   await _cacheService.RemoveAsync(CacheConstants.RecentComments);

                //Send mail
                //if (comment.ReplyId.HasValue)
                //{
                //    var repliedComment = await _context.Comments.FindAsync(comment.ReplyId.Value);
                //    var repledUser = await _context.AspNetUsers.FindAsync(repliedComment.OwnerUserId);
                //    var emailModel = new RepliedCommentVm()
                //    {
                //        CommentContent = request.Content,
                //        KnowledeBaseId = knowledgeBaseId,
                //        KnowledgeBaseSeoAlias = knowledgeBase.SeoAlias,
                //        KnowledgeBaseTitle = knowledgeBase.Title,
                //        RepliedName = repledUser.FirstName + " " + repledUser.LastName
                //    };
                //https://github.com/leemunroe/responsive-html-email-template
                //var htmlContent = await _viewRenderService.RenderToStringAsync("_RepliedCommentEmail", emailModel);
                //  await _emailSender.SendEmailAsync(repledUser.Email, "Có người đang trả lời bạn", htmlContent);
                //}
                //return Ok();
                return Ok(result);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Create comment failed! Server Error"));
            }
        }

        [Route("{knowledgeBaseId}/comments/{commentId}")]
        //[ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.UPDATE)]
        //[ApiValidationFilter]
        [HttpPut]
        public async Task<IHttpActionResult> PutComment(int commentId, [FromBody] CommentCreateRequest request)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, $"Cannot found comment with id: {commentId}"));
            if (comment.OwnerUserId != Guid.NewGuid().ToString())
                return Content(HttpStatusCode.Forbidden, "");

            comment.Content = request.Content;
            _context.Comments.Attach(comment);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent, ""));
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent, $"Update comment failed! Server Error"));
        }

        [Route("{knowledgeBaseId}/comments/{commentId}")]
        [HttpDelete]
        //[ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.DELETE)]
        public async Task<IHttpActionResult> DeleteComment(int knowledgeBaseId, int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent, $"Cannot found the comment with id: {commentId}"));

            _context.Comments.Remove(comment);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, $"Cannot found knowledge base with id: {knowledgeBaseId}"));

            knowledgeBase.NumberOfComments = knowledgeBase.NumberOfComments.GetValueOrDefault(0) - 1;
            _context.KnowledgeBases.Attach(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                // TODO
                //Delete cache
                //await _cacheService.RemoveAsync(CacheConstants.RecentComments);
                //var commentVm = new CommentVm()
                //{
                //    Id = comment.Id,
                //    Content = comment.Content,
                //    CreateDate = comment.CreateDate,
                //    KnowledgeBaseId = comment.KnowledgeBaseId,
                //    LastModifiedDate = comment.LastModifiedDate,
                //    OwnerUserId = comment.OwnerUserId
                //};
                //return Ok(commentVm);
                return Ok(result);
            }
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, $"Delete comment failed! Server Error"));
        }
    }
}
