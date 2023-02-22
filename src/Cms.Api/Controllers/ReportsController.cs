using Cms.Api.Data;
using Cms.Api.Models;
using Cms.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Cms.Api.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportsController : ApiController
    {
        private readonly CmsDbContext _context = new CmsDbContext();
        #region Reports
        [Route("{knowledgeBaseId}/reports/filter")]
        [HttpGet]
        public async Task<IHttpActionResult> GetReportsPaging(int? knowledgeBaseId, string filter, int pageIndex, int pageSize)
        {
            var query = from r in _context.Reports
                        join u in _context.AspNetUsers
                            on r.ReportUserId equals u.Id
                        select new { r, u };
            if (knowledgeBaseId.HasValue)
            {
                query = query.Where(x => x.r.KnowledgeBaseId == knowledgeBaseId.Value);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.r.Content.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ReportVm()
                {
                    Id = c.r.Id,
                    Content = c.r.Content,
                    CreateDate = c.r.CreateDate,
                    KnowledgeBaseId = c.r.KnowledgeBaseId,
                    LastModifiedDate = c.r.LastModifiedDate,
                    IsProcessed = false,
                    ReportUserId = c.r.ReportUserId,
                    ReportUserName = c.u.FirstName + " " + c.u.LastName
                })
                .ToListAsync();

            var pagination = new Pagination<ReportVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }
        [Route("{knowledgeBaseId}/reports/{reportId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetReportDetail(int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return NotFound();
            var user = await _context.AspNetUsers.FindAsync(report.ReportUserId);

            var reportVm = new ReportVm()
            {
                Id = report.Id,
                Content = report.Content,
                CreateDate = report.CreateDate,
                KnowledgeBaseId = report.KnowledgeBaseId,
                LastModifiedDate = report.LastModifiedDate,
                IsProcessed = report.IsProcessed,
                ReportUserId = report.ReportUserId,
                ReportUserName = user.FirstName + " " + user.LastName
            };
            return Ok(reportVm);
        }

        [Route("{knowledgeBaseId}/reports")]
        [HttpPost]
        public async Task<IHttpActionResult> PostReport(int knowledgeBaseId, [FromBody] ReportCreateRequest request)
        {
            var report = new Report()
            {
                Content = request.Content,
                KnowledgeBaseId = knowledgeBaseId,
                ReportUserId = Guid.NewGuid().ToString(),  // TODO: User.GetUserId(),
                IsProcessed = false
            };
            _context.Reports.Add(report);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Cannot found knowledge base with id {knowledgeBaseId}"));

            knowledgeBase.NumberOfReports = knowledgeBase.NumberOfReports.GetValueOrDefault(0) + 1;
            _context.KnowledgeBases.Attach(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Create report failed ! Server error"));
            }
        }

        [Route("{knowledgeBaseId}/reports/{reportId}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteReport(int knowledgeBaseId, int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Cannot found report with id {reportId}"));

            _context.Reports.Remove(report);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Cannot found knowledge base with id {knowledgeBaseId}"));

            knowledgeBase.NumberOfReports = knowledgeBase.NumberOfReports.GetValueOrDefault(0) - 1;
            _context.KnowledgeBases.Attach(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Delete report failed! Server error"));
        }

        #endregion Reports
    }
}
