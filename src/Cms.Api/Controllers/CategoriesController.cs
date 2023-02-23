using Cms.Api.Data;
using Cms.Api.Models;
using Cms.ViewModels;
using Serilog;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Cms.Api.Controllers
{
    [RoutePrefix("api/categories")]
    public class CategoriesController : ApiController
    {
        private readonly CmsDbContext _context = new CmsDbContext();
        // GET api/values

        [HttpGet]
        [ResponseType(typeof(List<CategoryVm>))]
        public async Task<IHttpActionResult> GetCategories()
        {
            Log.Information("Start log category");
            var categorys = await _context.Categories.ToListAsync();

            var categoryvms = categorys.Select(c => CreateCategoryVm(c)).ToList();
            Log.Information("End log category");
            return Ok(categoryvms);
        }

        // GET api/values/5
        [Route("{id:int}")]
        [HttpGet]
        [ResponseType(typeof(CategoryVm))]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            var categoryVm = CreateCategoryVm(category);
            return Ok(categoryVm);
        }
        // GET api/values/5
        [Route("filter")]
        [HttpGet]
        [ResponseType(typeof(Pagination<CategoryVm>))]
        public async Task<IHttpActionResult> GetCategoriesPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Categories.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Name.Contains(filter)
                || x.Name.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.OrderBy(x => x.SortOrder).Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            var data = items.Select(c => CreateCategoryVm(c)).ToList();

            var pagination = new Pagination<CategoryVm>
            {
                Items = data,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        // POST api/values
        [HttpPost]
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> PostCategories(CategoryCreateRequest request)
        {
            var category = new Category()
            {
                Name = request.Name,
                SeoAlias = request.SeoAlias,
                SeoDescription = request.SeoDescription,
                ParentId = request.ParentId,
                SortOrder = request.SortOrder
            };

            _context.Categories.Add(category);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return Ok(category);
            else
                return BadRequest();
        }

        // PUT api/values/5
        [Route("{id:int}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutCategory(int id, [FromBody] CategoryCreateRequest request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();
            if (id == request.ParentId)
                return BadRequest("Category cannot be a child itself.");

            category.Name = request.Name;
            category.ParentId = request.ParentId;
            category.SortOrder = request.SortOrder;
            category.SeoAlias = request.SeoAlias;
            category.SeoDescription = request.SeoDescription;

            _context.Categories.Attach(category);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }

        // DELETE api/values/5
        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var categoryvm = CreateCategoryVm(category);
                return Ok(categoryvm);
            }
            return BadRequest();
        }

        #region Hàm hỗ trợ
        private static CategoryVm CreateCategoryVm(Category category)
        {
            return new CategoryVm()
            {
                Id = category.Id,
                Name = category.Name,
                SortOrder = category.SortOrder,
                ParentId = category.ParentId,
                NumberOfTickets = category.NumberOfTickets,
                SeoDescription = category.SeoDescription,
                SeoAlias = category.SeoAlias
            };
        }
        #endregion
    }
}
