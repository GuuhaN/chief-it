using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using yak_shop_api.Controllers;
using yak_shop_api.Models;
using System.Threading.Tasks;
using System;

namespace yak_shop_api.UnitTests
{
    public class YakShopControllerTest
    {
        private DbContextOptions<YakShopContext> dbOptions;
        private YakShopContext yakShopContext;
        YakShopController controllerTest;
        public YakShopControllerTest()
        {
            dbOptions = new DbContextOptionsBuilder<YakShopContext>().UseInMemoryDatabase(databaseName: "YakshopTestDB").Options;
            yakShopContext = new YakShopContext(dbOptions);
            controllerTest = new YakShopController(yakShopContext);
        }

        [Fact]
        public async Task CreateHerdOfOneValid()
        {
            Herd herd = new Herd
            {
                Id = 1,
                Yaks = new List<Yak> {
                    new Yak {
                    Id = 1, Age = 1, Name = "Testing", Sex = "MALE"}
                }
            };
            await controllerTest.PostHerd(herd);
            yakShopContext.SaveChanges();
            Assert.NotEmpty(controllerTest.GetHerds().Value.ToList()[0].Yaks);
        }

        [Fact]
        public async Task CreateHerdOfOneInvalid10Years()
        {
            Herd herd = new Herd
            {
                Id = 1,
                Yaks = new List<Yak> {
                    new Yak {
                    Id = 1, Age = 10, Name = "Testing", Sex = "MALE"}
                }
            };
            await controllerTest.PostHerd(herd);
            yakShopContext.SaveChanges();
            Assert.Empty(controllerTest.GetHerds().Value.ToList()[0].Yaks);
        }

        [Fact]
        public async Task CheckStockOf13DaysOfSample()
        {
            Herd herd = new Herd
            {
                Id = 1,
                Yaks = new List<Yak> {
                    new Yak {
                    Id = 1, Age = 4, Name = "Billy", Sex = "MALE"},
                    new Yak {
                    Id = 2, Age = 8, Name = "Tilly", Sex = "FEMALE"},
                    new Yak {
                    Id = 3, Age = 9.5, Name = "Villy", Sex = "FEMALE"}
                }
            };
            await controllerTest.PostHerd(herd);
            yakShopContext.SaveChanges();

            Assert.Equal(1104.48, Math.Round(controllerTest.GetStock(13).Value.Milk,2));
            Assert.Equal(3, controllerTest.GetStock(13).Value.Skins);
        }
    }
}
