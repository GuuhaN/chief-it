using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using yak_shop_api.Controllers;
using yak_shop_api.Models;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace yak_shop_api.UnitTests
{
    public class YakShopControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private DbContextOptions<YakShopContext> dbOptions;
        private YakShopContext yakShopContext;
        YakShopController controllerTest;
        HttpClient httpClient;

        public YakShopControllerTest(WebApplicationFactory<Startup> fixture)
        {
            dbOptions = new DbContextOptionsBuilder<YakShopContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            httpClient = fixture.CreateClient();
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
                Id = 2,
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
                Id = 3,
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

        [Fact]
        public async Task GetPartialResponseOfStockMilkNoSkins206()
        {
            Herd herd = new Herd
            {
                Id = 4,
                Yaks = new List<Yak> {
                    new Yak {
                    Id = 1, Age = 4, Name = "Billy", Sex = "MALE"}
                }
            };

            CustomerOrder orderTooMuchSkin = new CustomerOrder
            {
                Id = 1,
                Customer = "Nigel",
                Order = new OrderItem
                {
                    Id = 1,
                    Milk = 100,
                    Skins = 100000
                }
            };

            await controllerTest.PostHerd(herd);
            yakShopContext.SaveChanges();

            var aResult = controllerTest.PostOrder(13, orderTooMuchSkin).Result.Result;
            ObjectResult createdResult = aResult as ObjectResult;
            CustomerOrder receivedOrder = createdResult.Value as CustomerOrder;

            Assert.Equal(206, createdResult.StatusCode);
            Assert.Equal(100, receivedOrder.Order.Milk);
            Assert.Null(receivedOrder.Order.Skins);
        }

        [Fact]
        public async Task GetPartialResponseOfStockNoMilkAndNoSkins404()
        {
            Herd herd = new Herd
            {
                Id = 5,
                Yaks = new List<Yak> {
                    new Yak {
                    Id = 1, Age = 4, Name = "Billy", Sex = "MALE"}
                }
            };

            CustomerOrder orderTooMuchSkin = new CustomerOrder
            {
                Id = 2,
                Customer = "Nigel",
                Order = new OrderItem
                {
                    Id = 1,
                    Milk = 10000000,
                    Skins = 10000000
                }
            };

            await controllerTest.PostHerd(herd);
            yakShopContext.SaveChanges();

            var aResult = controllerTest.PostOrder(13, orderTooMuchSkin).Result.Result;
            ObjectResult createdResult = aResult as ObjectResult;

            Assert.Equal(404, createdResult.StatusCode);
        }
    }
}
