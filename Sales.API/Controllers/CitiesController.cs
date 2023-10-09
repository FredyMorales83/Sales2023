using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.Helpers;
using Sales.Shared.DTOs;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly DataContext _context;

        public CitiesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Cities
                .Where(x => x.State!.Id == pagination.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return Ok(await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Cities
                .Where(x => x.State!.Id == pagination.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(City city)
        {
            try
            {
                _context.Add(city);
                await _context.SaveChangesAsync();

                return Ok(city);
            }
            catch (DbUpdateException uex)
            {
                if (uex.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe una ciudad con el mismo nombre");
                }

                return BadRequest(uex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(City city)
        {
            try
            {
                _context.Update(city);
                await _context.SaveChangesAsync();

                return Ok(city);
            }
            catch (DbUpdateException uex)
            {
                if (uex.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe una ciudad con el mismo nombre");
                }

                return BadRequest(uex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var state = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id);

            if (state == null)
            {
                return NotFound();
            }

            return Ok(state);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
            {
                return NotFound();
            }

            _context.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}