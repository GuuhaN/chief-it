using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yak_shop_api;
using yak_shop_api.Models;

namespace yak_shop_api.Controllers
{
    [Route("yak-shop")]
    [ApiController]
    [EnableCors("Cors")]
    public class YakShopController : ControllerBase
    {
        private readonly YakShopContext _context;

        public YakShopController(YakShopContext context)
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
            Stock stockNow = GiveStock(day);
            _context.Stocks.RemoveRange(_context.Stocks);
            _context.Stocks.Add(stockNow);
            _context.SaveChanges();
            return stockNow;
        }

        [HttpGet("herd/{day}")]
        public ActionResult<IEnumerable<Herd>> GetHerdDays(int day)
        {
            var herd = _context.Herds.Include(x => x.Yaks).ToList();
            if (herd.FirstOrDefault() == null || herd == null)
                return NotFound("There is no herd, please initiliaze one.");

            List<Yak> yaks = herd.First().Yaks;
            foreach (Yak yak in yaks)
            {
                yak.AgeLastShaved = yak.Age;
                for (int i = 0; i < day; i++)
                {
                    double newAge = yak.Age + (double)i / 100;
                    if (CanShave(newAge, (double)yak.AgeLastShaved))
                        yak.AgeLastShaved = newAge;
                }
                yak.Age = yak.Age + (double)day / 100;
            }
            return herd;
        }

        // POST: api/Herds
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("load")]
        [ProducesResponseType(StatusCodes.Status205ResetContent)]
        public async Task<ActionResult<Herd>> PostHerd(Herd herd)
        {
            _context.Herds.RemoveRange(_context.Herds);
            for (int i = 0; i < herd.Yaks.Count; i++)
            {
                if (herd.Yaks[i].Age >= 10)
                    herd.Yaks.RemoveAt(i);
                herd.Yaks[i].AgeLastShaved = null;
            }
            _context.Herds.Add(herd);
            await _context.SaveChangesAsync();
            return herd;
        }

        [HttpPost("order/{day}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CustomerOrder))]
        [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(CustomerOrder))]
        public async Task<ActionResult<CustomerOrder>> PostOrder(int day, CustomerOrder customerOrder)
        {
            Stock inStock = GiveStock(day);
            double? preOrderMilk = customerOrder.Order.Milk;
            int? preOrderSkins = customerOrder.Order.Skins;
            if (preOrderMilk > inStock.Milk)
                preOrderMilk = null;

            if (preOrderSkins > inStock.Skins)
                preOrderSkins = null;
            CustomerOrder order = new CustomerOrder
            {
                Customer = customerOrder.Customer,
                Order = new OrderItem
                {
                    Milk = preOrderMilk,
                    Skins = preOrderSkins
                }
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            if (order.Order.Milk == null && order.Order.Milk == null)
            {
                return NotFound("No stock of your order");
            }

            if (order.Order.Milk == null || order.Order.Milk == null)
            {
                return NotFound(order); // Moet een 206 Partial Content returnen.
            }

            return order;
        }

        private bool CanShave(double currentAge, double lastAgeShaved)
        {
            if(currentAge >= 1)
                return currentAge >= lastAgeShaved + ((8 + currentAge) / 100);

            return false;
        }

        private Stock GiveStock(int day)
        {
            var herd = _context.Herds.Include(x => x.Yaks).ToList();
            if (herd.FirstOrDefault() == null || herd == null)
                return null;

            List<Yak> yaks = herd.First().Yaks;
            double totalMilk = 0;
            int totalSkins = 0;
            // days * (50 - (age of one yak * 0.03)) = Liters of milk
            foreach (Yak yak in yaks)
            {
                if (yak.Age < 10)
                {
                    if (yak.Age > 1)
                        totalSkins++;
                    yak.AgeLastShaved = yak.Age;
                    for (int i = 0; i < day; i++)
                    {
                        double newAge = yak.Age + (double)i / 100;
                        totalMilk += 50 - ((newAge * 100) * 0.03);
                        if (CanShave(newAge, (double)yak.AgeLastShaved))
                        {
                            yak.AgeLastShaved = newAge;
                            totalSkins++;
                        }
                    }
                }
            }
            Stock stockNow = new Stock
            {
                Milk = totalMilk,
                Skins = totalSkins
            };
            return stockNow;
        }

        //[HttpGet("/getherd/{id}")]
        //public ActionResult<Herd> GetHerdInfo(long id)
        //{
        //    var herd = _context.Herds.Include(x => x.Yaks).FirstOrDefault(x => x.Id == id);

        //    if (herd == null)
        //        return NotFound();

        //    return herd;
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
