using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatesController : ControllerBase
    {
        private readonly DataContext _context;

        public StatesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.States.Include(c => c.Cities).OrderBy(c => c.Id).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var state = await _context.States.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Id == id);

            if (state == null)
            {
                return NotFound();
            }

            return Ok(state);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(State state)
        {
            try
            {
                _context.Add(state);
                await _context.SaveChangesAsync();

                return Ok(state);
            }
            catch (DbUpdateException uex)
            {
                if (uex.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un estado con el mismo nombre");
                }

                return BadRequest(uex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(State state)
        {
            try
            {
                _context.Update(state);
                await _context.SaveChangesAsync();

                return Ok(state);
            }
            catch (DbUpdateException uex)
            {
                if (uex.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un estado con el mismo nombre");
                }

                return BadRequest(uex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var state = await _context.States.FirstOrDefaultAsync(c => c.Id == id);

            if (state == null)
            {
                return NotFound();
            }

            _context.Remove(state);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}