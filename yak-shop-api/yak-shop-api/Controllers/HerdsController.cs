using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yak_shop_api;
using yak_shop_api.Models;

namespace yak_shop_api.Controllers
{
    [Route("yak-shop")]
    [ApiController]
    public class HerdsController : ControllerBase
    {
        private readonly HerdContext _context;

        public HerdsController(HerdContext context)
        {
            _context = context;
        }

        // GET: api/Herds
        [HttpGet("TESTING")]
        public ActionResult<IEnumerable<Herd>> GetHerds()
        {
            return _context.Herds.Include(x=> x.Yaks).ToList();
        }

        [HttpGet("stock/{day}")]
        public ActionResult<Stock> GetStock(int day)
        {
            var herd = _context.Herds.Include(x=>x.Yaks);
            if(herd.FirstOrDefault() == null)
                return NotFound();

            int yaksCount = herd.First().Yaks.Count;
            Stock stockNow = new Stock { Milk = day * (yaksCount * (50 - (day * 0.03))), Skins = day * (yaksCount * (8 + (day * 0.01)))};
            return stockNow;
        }

        // GET: api/Herds/5
        [HttpGet("herd/get/{id}")]
        public ActionResult<Herd> GetHerdInfo(long id)
        {
            var herd = _context.Herds.Include(x => x.Yaks).FirstOrDefault(x => x.Id == id);

            if (herd == null)
            {
                return NotFound();
            }

            return herd;
        }

        [HttpGet("herd/{day}")]
        public ActionResult<IEnumerable<Herd>> GetHerdDays(double day)
        {
            var herd = _context.Herds.Include(x => x.Yaks).ToList();
            for (int i = 0; i < herd.FirstOrDefault().Yaks.Count; i++)
            {
                herd.FirstOrDefault().Yaks[i].Age = herd.FirstOrDefault().Yaks[i].Age + day / 100;
            }

            if (herd == null)
            {
                return NotFound();
            }

            return herd;
        }

        // POST: api/Herds
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("load")]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(Herd))]
        public async Task<ActionResult<Herd>> PostHerd(Herd herd)
        {
            _context.Herds.RemoveRange(_context.Herds);
            for (int i = 0; i < herd.Yaks.Count; i++)
            {
                herd.Yaks[i].AgeLastShaved = herd.Yaks[i].Age;
            }
            _context.Herds.Add(herd);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHerdInfo), new { id = herd.Id }, herd);
        }

        //[HttpPost("order/{day}")]
        //public async Task<ActionResult<CustomerOrder>> PostOrder(int day)
        //{
        //    var order = _context.Orders;

        //    return Create
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteHerd(long id)
        //{
        //    var herd = await _context.Herds.FindAsync(id);
        //    if (herd == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Herds.Remove(herd);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool HerdExists(long id)
        {
            return _context.Herds.Any(e => e.Id == id);
        }
    }
}
