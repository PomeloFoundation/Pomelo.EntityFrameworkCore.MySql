using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Controllers
{
    [Route("api/[controller]")]
    public class AsyncController : Controller
    {
	    private readonly AppDb _db;

	    public AsyncController(AppDb db)
	    {
		    _db = db;
	    }

        // GET api/async
        [HttpGet]
        public async Task<IActionResult> Get()
        {
			return new ObjectResult(await _db.Blogs.Include(m => m.Posts).OrderByDescending(m => m.Id).Take(10).ToListAsync());
        }

        // GET api/async/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
			var model = await _db.Blogs.Include(m => m.Posts).FirstOrDefaultAsync(m => m.Id == id);
			if (model != null)
			{
				return new ObjectResult(model);
			}
			return NotFound();
        }

        // POST api/async
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Blog body)
        {
	        _db.Blogs.Add(body);
			await _db.SaveChangesAsync();
			return new ObjectResult(body);
        }

        // PUT api/async/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Blog body)
        {
			var model = await _db.Blogs.Include(m => m.Posts).FirstOrDefaultAsync(m => m.Id == id);
			if (model != null)
			{
				model.Title = body.Title;
				model.Posts = body.Posts;
				await _db.SaveChangesAsync();
				return new ObjectResult(model);
			}
			return NotFound();
        }

        // DELETE api/async/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
			var model = await _db.Blogs.FirstOrDefaultAsync(m => m.Id == id);
			if (model != null)
			{
				_db.Blogs.Remove(model);
				await _db.SaveChangesAsync();
				return Ok();
			}
			return NotFound();
        }
    }
}
