using System.Collections.Generic;
using System.Linq;
using APILibrary.Test.Mock;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using WebApplication.Controllers;
using WebApplication.Models;

namespace APILibrary.Test
{
    public class Tests
    {
        private MockDbContext _db;
        private PizzasController _controllerP;
        private CustomersController _controllerC;

        [SetUp]
        public void Setup()
        {
            _db = MockDbContext.GetDbContext();
            _controllerP = new PizzasController(_db);
            _controllerC = new CustomersController(_db);
        }

        [Test]
        public async System.Threading.Tasks.Task RetournerStatutOkNombrePizza()
        {
            var actionResult = await _controllerP.GetAllAsync("", "", "","");
            var result = actionResult.Result as ObjectResult;
            var values = ((IEnumerable<object>)(result).Value);

            Assert.AreEqual((int)System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(_db.Pizzas.Count(), values.Count());
        }

        [Test]
        public async System.Threading.Tasks.Task RetournerStatutOkNombreCustomer()
        {
            var actionResult = await _controllerC.GetAllAsync("", "", "", "");
            var result = actionResult.Result as ObjectResult;
            var values = ((IEnumerable<object>)(result).Value);

            Assert.AreEqual((int)System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(_db.Customers.Count(), values.Count());
        }

        [TestCase(1)]
        public async System.Threading.Tasks.Task RetournerStatutOkRecherchePizzaById(int id)
        {
            var actionResult = await _controllerC.GetById(id, "");
            var result = actionResult.Result as ObjectResult;

            Assert.AreEqual((int)System.Net.HttpStatusCode.OK, result.StatusCode);
        }

        [TestCase(2)]
        public async System.Threading.Tasks.Task RetournerStatutOkRechercheCustomerById(int id)
        {
            var actionResult = await _controllerC.GetById(id, "");
            var result = actionResult.Result as ObjectResult;

            Assert.AreEqual((int)System.Net.HttpStatusCode.OK, result.StatusCode);
        }

    }
}