using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Controllers
{
    [Route("api/[controller]")]
    public class SyncController : Controller
    {
	    private readonly AppDb _db;

	    public SyncController(AppDb db)
	    {
		    _db = db;
	    }

	    // GET api/sync
        [HttpGet]
        public IActionResult Get()
        {
			return new ObjectResult(_db.Blogs.Include(m => m.Posts).OrderByDescending(m => m.Id).Take(10).ToList());
        }

        // GET api/sync/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
			var model = _db.Blogs.Include(m => m.Posts).FirstOrDefault(m => m.Id == id);
			if (model != null)
			{
				return new ObjectResult(model);
			}
			return NotFound();
        }

        // POST api/sync
        [HttpPost]
        public IActionResult Post([FromBody] Blog body)
        {
	        _db.Blogs.Add(body);
	        _db.SaveChanges();
			return new ObjectResult(body);
        }

        // PUT api/sync/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Blog body)
        {
			var model = _db.Blogs.Include(m => m.Posts).FirstOrDefault(m => m.Id == id);
			if (model != null)
			{
				model.Title = body.Title;
				model.Posts = body.Posts;
				_db.SaveChanges();
				return new ObjectResult(model);
			}
			return NotFound();
        }

        // DELETE api/sync/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
			var model = _db.Blogs.FirstOrDefault(m => m.Id == id);
			if (model != null)
			{
				_db.Blogs.Remove(model);
				_db.SaveChanges();
				return Ok();
			}
			return NotFound();
        }
    }
}
