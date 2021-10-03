using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;
using yak_shop_api.Controllers;
using yak_shop_api.Models;

namespace yak_shop_api.UnitTests
{
    public class YakShopControllerTest
    {
        private YakShopController yakShopController;
        DbContextOptions<YakShopContext> contextOptions;
        public YakShopControllerTest(DbContextOptions<YakShopContext> options)
        {
            contextOptions = options;
        }
        [Fact]
        public void GetHerdTest()
        {
            using(var context = new YakShopContext(contextOptions))
            {
                yakShopController = new YakShopController(context);
                var herd = yakShopController.GetHerds().Value.ToList();

                Assert.Equal(1, herd[0].Id);
            }
        }
    }
}
