using Cms.Api.Data;
using Cms.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Cms.Api.Controllers
{
    [RoutePrefix("api/votes")]
    public class VotesController : ApiController
    {
        private readonly CmsDbContext _context = new CmsDbContext();
        #region Votes

        [Route("{knowledgeBaseId}/votes")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVotes(int knowledgeBaseId)
        {
            var votes = await _context.Votes
                .Where(x => x.KnowledgeBaseId == knowledgeBaseId)
                .Select(x => new VoteVm()
                {
                    UserId = x.UserId,
                    KnowledgeBaseId = x.KnowledgeBaseId,
                    CreateDate = x.CreateDate,
                    LastModifiedDate = x.LastModifiedDate
                }).ToListAsync();
            return Ok(votes);
        }
        [Route("{knowledgeBaseId}/votes")]
        [HttpPost]
        public async Task<IHttpActionResult> PostVote(int knowledgeBaseId)
        {
            var userId = Guid.NewGuid().ToString(); //User.GetUserId();
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Cannot found knowledge base with id {knowledgeBaseId}"));

            var numberOfVotes = await _context.Votes.CountAsync(x => x.KnowledgeBaseId == knowledgeBaseId);
            var vote = await _context.Votes.FindAsync(knowledgeBaseId, userId);
            if (vote != null)
            {
                _context.Votes.Remove(vote);
                numberOfVotes -= 1;
            }
            else
            {
                vote = new Vote()
                {
                    KnowledgeBaseId = knowledgeBaseId,
                    UserId = userId
                };
                _context.Votes.Add(vote);
                numberOfVotes += 1;
            }

            knowledgeBase.NumberOfVotes = numberOfVotes;
            _context.KnowledgeBases.Attach(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok(numberOfVotes);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Vote failed"));
            }
        }
        [Route("{knowledgeBaseId}/votes/{userId}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteVote(int knowledgeBaseId, string userId)
        {
            var vote = await _context.Votes.FindAsync(knowledgeBaseId, userId);
            if (vote == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cannot found vote"));

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase != null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Cannot found knowledge base with id {knowledgeBaseId}"));

            knowledgeBase.NumberOfVotes = knowledgeBase.NumberOfVotes.GetValueOrDefault(0) - 1;
            _context.KnowledgeBases.Attach(knowledgeBase);

            _context.Votes.Remove(vote);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Delete vote failed! Server error"));
        }

        #endregion Votes

    }
}
