using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Controllers
{
    [Route("api/[controller]")]
    public class AsyncController : Controller
    {
        // GET api/async
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (var db = new AppDb())
            {
                return new ObjectResult(await db.Blogs.Include(m => m.Posts).OrderByDescending(m => m.Id).Take(10).ToListAsync());
            }
        }

        // GET api/async/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using (var db = new AppDb())
            {
                var model = await db.Blogs.Include(m => m.Posts).FirstOrDefaultAsync(m => m.Id == id);
                if (model != null)
                {
                    return new ObjectResult(model);
                }
                return NotFound();
            }
        }

        // POST api/async
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Blog body)
        {
            using (var db = new AppDb())
            {
                db.Blogs.Add(body);
                await db.SaveChangesAsync();
                return new ObjectResult(body);
            }
        }

        // PUT api/async/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Blog body)
        {
            using (var db = new AppDb())
            {
                var model = await db.Blogs.Include(m => m.Posts).FirstOrDefaultAsync(m => m.Id == id);
                if (model != null)
                {
                    model.Title = body.Title;
                    model.Posts = body.Posts;
                    await db.SaveChangesAsync();
                    return new ObjectResult(model);
                }
                return NotFound();
            }
        }

        // DELETE api/async/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (var db = new AppDb())
            {
                var model = await db.Blogs.FirstOrDefaultAsync(m => m.Id == id);
                if (model != null)
                {
                    db.Blogs.Remove(model);
                    await db.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
        }
    }
}
