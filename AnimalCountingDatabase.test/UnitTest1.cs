using AnimalCountingDatabase.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AnimalCountingDatabase.test
{
    public class DemoTests
    {
        [Fact]
        public void Test1()
        {
            Assert.True(1 == 1);
        }

        [Fact]
        public async Task CustomerIntergrationTest()
        {
            //Create DB Context
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build(); ;

            var optionsBuilder = new DbContextOptionsBuilder<CustomerContext>();
            optionsBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

            var context = new CustomerContext(optionsBuilder.Options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            //Just to make sure: Delete all existing custoemrs in the database
            context.Customers.RemoveRange(await context.Customers.ToArrayAsync());
            await context.SaveChangesAsync();

            //Create Controller
            var controller = new CustomersController(context);

            //Add Cutsomer

            await controller.Add(new Customer() { CustomerName = "FooBar" });


            //Check: Does GetAll return the added customer?

            var result = (await controller.GetAll()).ToArray();
            //Check only one result was created
            Assert.Single(result);
            //Check that the customer object is correct
            Assert.Equal("FooBar", result[0].CustomerName);

        }
    }
}
