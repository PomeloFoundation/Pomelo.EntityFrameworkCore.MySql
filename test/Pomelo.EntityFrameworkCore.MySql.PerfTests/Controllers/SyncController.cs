using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Controllers
{
    [Route("api/[controller]")]
    public class SyncController : Controller
    {
        // GET api/sync
        [HttpGet]
        public IActionResult Get()
        {
            using (var db = new AppDb())
            {
                return new ObjectResult(db.Blogs.Include(m => m.Posts).OrderByDescending(m => m.Id).Take(100).ToList());
            }
        }

        // GET api/sync/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (var db = new AppDb())
            {
                var model = db.Blogs.Include(m => m.Posts).FirstOrDefault(m => m.Id == id);
                if (model != null)
                {
                    return new ObjectResult(model);
                }
                return NotFound();
            }
        }

        // POST api/sync
        [HttpPost]
        public IActionResult Post([FromBody] Blog body)
        {
            using (var db = new AppDb())
            {
                db.Blogs.Add(body);
                db.SaveChangesAsync();
                return new ObjectResult(body);
            }
        }

        // PUT api/sync/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Blog body)
        {
            using (var db = new AppDb())
            {
                var model = db.Blogs.Include(m => m.Posts).FirstOrDefault(m => m.Id == id);
                if (model != null)
                {
                    model.Title = body.Title;
                    model.Posts = body.Posts;
                    db.SaveChanges();
                    return new ObjectResult(model);
                }
                return NotFound();
            }
        }

        // DELETE api/sync/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (var db = new AppDb())
            {
                var model = db.Blogs.FirstOrDefault(m => m.Id == id);
                if (model != null)
                {
                    db.Blogs.Remove(model);
                    db.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
        }
    }
}
