using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yak_shop_api.Models;

namespace yak_shop_api
{
    public class YakShopContext : DbContext
    {
        public YakShopContext(DbContextOptions<YakShopContext> options ): base(options) { }

        public DbSet<Herd> Herds { get; set; }
        public DbSet<Yak> Yaks { get; set; }
        public DbSet<CustomerOrder> Orders { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
