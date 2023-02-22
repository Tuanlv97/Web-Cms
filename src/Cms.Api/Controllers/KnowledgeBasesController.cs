using Cms.Api.Data;
using Cms.Api.Helpers;
using Cms.Api.Models;
using Cms.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Cms.Api.Controllers
{
    [RoutePrefix("api/knowledgeBases")]
    public class KnowledgeBasesController : ApiController
    {
        private readonly CmsDbContext _context = new CmsDbContext();

        [Route("all")]
        [HttpGet]
        public async Task<IHttpActionResult> GetKnowledgeBases()
        {
            var knowledgeBases = _context.KnowledgeBases;
            var knowledgeBasevms = await knowledgeBases.Select(u => new KnowledgeBaseQuickVm()
            {
                Id = u.Id,
                CategoryId = u.CategoryId,
                Description = u.Description,
                SeoAlias = u.SeoAlias,
                Title = u.Title
            }).ToListAsync();
            return Ok(knowledgeBasevms);
        }

        [Route("filter")]
        [HttpGet]
        [ResponseType(typeof(Pagination<KnowledgeBaseQuickVm>))]
        public async Task<IHttpActionResult> GetKnowledgeBasesPaging(string filter, int pageIndex, int pageSize)
        {
            var query = from k in _context.KnowledgeBases
                        join c in _context.Categories on k.CategoryId equals c.Id
                        select new { k, c };
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.k.Title.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.OrderBy(x => x.k.CreateDate).Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new KnowledgeBaseQuickVm()
                {
                    Id = u.k.Id,
                    CategoryId = u.k.CategoryId,
                    Description = u.k.Description,
                    SeoAlias = u.k.SeoAlias,
                    Title = u.k.Title,
                    CategoryName = u.c.Name
                })
                .ToListAsync();

            var pagination = new Pagination<KnowledgeBaseQuickVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [Route("{id:int}")]
        [HttpGet]
        [ResponseType(typeof(KnowledgeBaseVm))]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
            if (knowledgeBase == null)
                return NotFound();

            var attachments = await _context.Attachments
                .Where(x => x.KnowledgeBaseId == id)
                .Select(x => new AttachmentVm()
                {
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    FileSize = x.FileSize,
                    Id = x.Id,
                    FileType = x.FileType
                }).ToListAsync();
            var knowledgeBaseVm = CreateKnowledgeBaseVm(knowledgeBase);
            knowledgeBaseVm.Attachments = attachments;
            return Ok(knowledgeBaseVm);
        }

        [HttpPost]
        //[Consumes("multipart/form-data")] [FromForm] 
        public async Task<IHttpActionResult> PostKnowledgeBase(KnowledgeBaseCreateRequest request)
        {
            KnowledgeBas knowledgeBase = CreateKnowledgeBaseEntity(request);
            knowledgeBase.OwnerUserId = "0f8fad5b-d9cb-469f-a165-70867728950e";
            if (string.IsNullOrEmpty(knowledgeBase.SeoAlias))
            {
                knowledgeBase.SeoAlias = TextHelper.ToUnsignString(knowledgeBase.Title);
            }
            //  knowledgeBase.Id = await _sequenceService.GetKnowledgeBaseNewId();

            //Process attachment
            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                foreach (var attachment in request.Attachments)
                {
                    var attachmentEntity = await SaveFile(knowledgeBase.Id, attachment);
                    _context.Attachments.Add(attachmentEntity);
                }
            }
            _context.KnowledgeBases.Add(knowledgeBase);

            //Process label
            if (request.Labels?.Length > 0)
            {
                await ProcessLabel(request, knowledgeBase);
            }

            var result = await _context.SaveChangesAsync();

            if (result > 0)

                return Ok();
            else

                return BadRequest();
        }
        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteKnowledgeBase(int id)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
            if (knowledgeBase == null)
                return NotFound();

            _context.KnowledgeBases.Remove(knowledgeBase);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                KnowledgeBaseVm knowledgeBasevm = CreateKnowledgeBaseVm(knowledgeBase);
                return Ok(knowledgeBasevm);
            }
            return BadRequest();
        }

        private static KnowledgeBaseVm CreateKnowledgeBaseVm(KnowledgeBas knowledgeBase)
        {
            return new KnowledgeBaseVm()
            {
                Id = knowledgeBase.CategoryId,

                CategoryId = knowledgeBase.CategoryId,

                Title = knowledgeBase.Title,

                SeoAlias = knowledgeBase.SeoAlias,

                Description = knowledgeBase.Description,

                Environment = knowledgeBase.Environment,

                Problem = knowledgeBase.Problem,

                StepToReproduce = knowledgeBase.StepToReproduce,

                ErrorMessage = knowledgeBase.ErrorMessage,

                Workaround = knowledgeBase.Workaround,

                Note = knowledgeBase.Note,

                OwnerUserId = knowledgeBase.OwnerUserId,

                Labels = !string.IsNullOrEmpty(knowledgeBase.Labels) ? knowledgeBase.Labels.Split(',') : null,

                CreateDate = knowledgeBase.CreateDate,

                LastModifiedDate = knowledgeBase.LastModifiedDate,

                NumberOfComments = knowledgeBase.CategoryId,

                NumberOfVotes = knowledgeBase.CategoryId,

                NumberOfReports = knowledgeBase.CategoryId,
            };
        }
        private static KnowledgeBas CreateKnowledgeBaseEntity(KnowledgeBaseCreateRequest request)
        {
            var entity = new KnowledgeBas()
            {
                CategoryId = request.CategoryId,

                Title = request.Title,

                SeoAlias = request.SeoAlias,

                Description = request.Description,

                Environment = request.Environment,

                Problem = request.Problem,

                StepToReproduce = request.StepToReproduce,

                ErrorMessage = request.ErrorMessage,

                Workaround = request.Workaround,

                Note = request.Note
            };
            if (request.Labels?.Length > 0)
            {
                entity.Labels = string.Join(",", request.Labels);
            }
            return entity;
        }

        private async Task<Attachment> SaveFile(int knowledegeBaseId, HttpPostedFileBase file)
        {
            const int maxFileLength = 15360; // 15KB = 1024 * 15
            var supportedTypes = new[] { "txt", "doc", "docx", "pdf", "xls", "xlsx" };
            var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
            if (!supportedTypes.Contains(fileExt))
            {
                // return Content(System.Net.HttpStatusCode.ExpectationFailed, "File Extension Is InValid - Only Upload WORD/PDF/EXCEL/TXT File");
            }
            var originalFileName = Path.GetFileName(file.FileName);
            var fileName = $"{originalFileName.Substring(0, originalFileName.LastIndexOf('.'))}{Path.GetExtension(originalFileName)}";

            long fileSize = file.ContentLength;

            if (fileSize > maxFileLength)
            {
                string message = $"Your post has a size of {fileSize} bytes which exceeded " +
                    $"the limit of {maxFileLength} bytes. Please upload a smaller file.";
            }

            var attachmentEntity = new Attachment()
            {
                FileName = fileName,
                FilePath = Path.Combine(
                HttpContext.Current.Server.MapPath("~/uploads"),
                fileName),
                FileSize = fileSize,
                FileType = Path.GetExtension(fileName),
                KnowledgeBaseId = knowledegeBaseId,
            };
            return attachmentEntity;
        }

        private async Task ProcessLabel(KnowledgeBaseCreateRequest request, KnowledgeBas knowledgeBase)
        {
            foreach (var labelText in request.Labels)
            {
                var labelId = TextHelper.ToUnsignString(labelText.ToString());
                var existingLabel = await _context.Labels.FindAsync(labelId);
                if (existingLabel == null)
                {
                    var labelEntity = new Label()
                    {
                        Id = labelId,
                        Name = labelText.ToString()
                    };
                    _context.Labels.Add(labelEntity);
                }
                if (await _context.LabelInKnowledgeBases.FindAsync(labelId, knowledgeBase.Id) == null)
                {
                    _context.LabelInKnowledgeBases.Add(new LabelInKnowledgeBas()
                    {
                        KnowledgeBaseId = knowledgeBase.Id,
                        LabelId = labelId
                    });
                }
            }
        }

        [Route("{id:int}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutKnowledgeBase(int id, KnowledgeBaseCreateRequest request)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
            if (knowledgeBase == null)
                return NotFound();

            //Process attachment
            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                foreach (var attachment in request.Attachments)
                {
                    var attachmentEntity = await SaveFile(knowledgeBase.Id, attachment);
                    _context.Attachments.Add(attachmentEntity);
                }
            }
            _context.KnowledgeBases.Attach(knowledgeBase);

            if (request.Labels?.Length > 0)
            {
                await ProcessLabel(request, knowledgeBase);
            }
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
