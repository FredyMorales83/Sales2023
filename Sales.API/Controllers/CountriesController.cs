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
    public class CountriesController : ControllerBase
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Countries
                .Include(x => x.States)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Filter))
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
            var queryable = _context.Countries.AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var country = await _context.Countries.Include(c => c.States!).ThenInclude(s => s.Cities).FirstOrDefaultAsync(c => c.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            return Ok(country);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Country country)
        {
            try
            {
                _context.Add(country);
                await _context.SaveChangesAsync();

                return Ok(country);
            }
            catch (DbUpdateException uex)
            {
                if (uex.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un pais con el mismo nombre");
                }

                return BadRequest(uex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(Country country)
        {
            try
            {
                _context.Update(country);
                await _context.SaveChangesAsync();

                return Ok(country);
            }
            catch (DbUpdateException uex)
            {
                if (uex.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un pais con el mismo nombre");
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
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            _context.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}