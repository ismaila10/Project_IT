using System;
using APILibrary.Test.Mock.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace APILibrary.Test.Mock
{
    public class MockDbContext: EatDbContext
    {
        public MockDbContext(DbContextOptions options) : base(options)
        {
        }

        public static MockDbContext GetDbContext(bool withData = true)
        {
            var options = new DbContextOptionsBuilder().UseInMemoryDatabase("dbtest").Options;
            var db = new MockDbContext(options);

            if (withData)
            {
                db.Pizzas.Add(new PizzaMock { Name = "Pizza 1", Price = 10, Topping = "Champignon" });
                db.Pizzas.Add(new PizzaMock { Name = "Pizza 2", Price = 11, Topping = "Champignon" });
                db.Pizzas.Add(new PizzaMock { Name = "Pizza 3", Price = 12, Topping = "Champignon" });

                db.Customers.Add(new CustomerMock { Email = "max@gmail.com", Phone = "0654788906", Lastname = "Fuente", Firstname = "Izaaac", Address = "Ivry", ZipCode = "12098", City = "NewYork" });
                db.Customers.Add(new CustomerMock { Email = "maxi@gmail.com", Phone = "0654786512", Lastname = "Rach", Firstname = "David", Address = "Choisy", ZipCode = "98065", City = "Chicago" });
                db.Customers.Add(new CustomerMock { Email = "maxim@gmail.com", Phone = "0654780432", Lastname = "Sidney", Firstname = "Jean", Address = "Paris", ZipCode = "45097", City = "USA" });

                db.SaveChanges();
            }

            return db;
        }
    }
}
