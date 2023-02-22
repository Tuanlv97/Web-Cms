using Cms.Api.Data;
using Cms.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Cms.Api.Controllers
{
    [RoutePrefix("api/labels")]
    public class LabelsController : ApiController
    {
        private readonly CmsDbContext _context = new CmsDbContext();

        [Route("{id}")]
        [HttpGet]
        [ResponseType(typeof(LabelVm))]
        public async Task<IHttpActionResult> GetById(string id)
        {
            var label = await _context.Labels.FindAsync(id);
            if (label == null)
                return NotFound();

            var labelVm = new LabelVm()
            {
                Id = label.Id,
                Name = label.Name
            };

            return Ok(labelVm);
        }
        [Route("popular/{take:int}")]
        [HttpGet]
        [ResponseType(typeof(LabelVm))]
        public async Task<List<LabelVm>> GetPopularLabels(int take)
        {
            return null;
            //var cachedData = await _cacheService.GetAsync<List<LabelVm>>(CacheConstants.PopularLabels);
            //if (cachedData == null)
            //{
            //    var query = from l in _context.Labels
            //                join lik in _context.LabelInKnowledgeBases on l.Id equals lik.LabelId
            //                group new { l.Id, l.Name } by new { l.Id, l.Name } into g
            //                select new
            //                {
            //                    g.Key.Id,
            //                    g.Key.Name,
            //                    Count = g.Count()
            //                };
            //    var labels = await query.OrderByDescending(x => x.Count).Take(take)
            //        .Select(l => new LabelVm()
            //        {
            //            Id = l.Id,
            //            Name = l.Name
            //        }).ToListAsync();
            //    await _cacheService.SetAsync(CacheConstants.PopularLabels, labels);
            //    cachedData = labels;
            //}

            //return cachedData;
        }
    }
}
