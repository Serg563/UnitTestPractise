using AutoMapper;
using Castle.Core.Resource;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using OrderApi;
using OrderApi.Data;
using OrderApi.DTO;
using OrderApi.Entities;
using OrderApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApiTests.IntegratedTests
{
    internal class OrderIntegrationTest
    {
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private const string RequestUri = "https://localhost:7151/api/OrderService/";


        [SetUp]
        public void Init()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task OrdererviceController_GetAllOrders_ReturnsAllFromDb()
        {
            //arrange
            var expected = GetTestOrderEntities.ToList();

            // act
            var httpResponse = await _client.GetAsync(RequestUri+ "GetAllOrders");

            // assert
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<IEnumerable<OrderGetAllModel>>(stringResponse).ToList();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task OrderServiceontroller_GetOrderById_ReturnsOrderById()
        {
            //arrange
            var expected = GetTestOrderModels.First();
            int orderId = 1;

            // act
            var httpResponse = await _client.GetAsync(RequestUri + $"GetOrderById/{orderId}");

            // assert
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Order>(stringResponse);

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task OrderServiceController_UpdateOrder_UpdatedOrder()
        {
            //arrange
            Order order = new Order
            {
                Id = 1,
                CustomerID = 1,
                EmployeeID = 1,
                OrderDate = new DateTime(2023, 01, 10),
                RequiredDate = new DateTime(2023, 01, 20),
                ShippedDate = new DateTime(2023, 01, 15),
                ShipVia = 1,
                Freight = 12.5,
                ShipName = "Order 1"
            };
            var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");

            // act
            var httpResponse = await _client.PutAsync(RequestUri + $"UpdateOrder",content);

            // assert
            httpResponse.EnsureSuccessStatusCode();
            await CheckCustomersInfoIntoDb(order.Id, 2);
        }

        [Test]
        public async Task OrderServiceController_AddOrder_AddedOrder()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfiler>());
            var mapper = config.CreateMapper();
            //arrange
            AddOrderDTO order = new AddOrderDTO
            {
                CustomerID = 1,
                EmployeeID = 1,
                ShipVia = 1,
                Freight = 12.5,
                ShipName = "Order 1"
            };
            var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");

            // act
            var httpResponse = await _client.PostAsync(RequestUri + $"AddOrder", content);

            // assert
            httpResponse.EnsureSuccessStatusCode();
            var mappedOrder = mapper.Map<Order>(order);
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var orderResponse = JsonConvert.DeserializeObject<AddOrderDTO>(stringResponse);
            await CheckCustomersInfoIntoDb(mappedOrder.Id, 3);
        }

        [Test]
        public async Task OrderServiceController_DeleteOrder_DeletedOrder()
        {
            //arrange
            var orderId = 2;
            var expectedLength = GetTestOrderModels.Count() - 1;

            // act
            var httpResponse = await _client.DeleteAsync(RequestUri + $"DeleteOrderById/{orderId}");

            // assert
            httpResponse.EnsureSuccessStatusCode();
            using (var test = _factory.Services.CreateScope())
            {
                var context = test.ServiceProvider.GetService<AppOrderContext>();
                context.Orders.Should().HaveCount(expectedLength);
            }
        }
        private static List<Order> GetTestOrderModels =>
           new List<Order>()
           {
                new Order { Id = 1, CustomerID = 1,Customer = null, EmployeeID = 1,Employee = null, OrderDate = new DateTime(2023, 01, 10), RequiredDate = new DateTime(2023, 01, 20), ShippedDate = new DateTime(2023, 01, 15), ShipVia = 1, Freight = 12.5, ShipName = "Order 1" },
                new Order { Id = 2, CustomerID = 2,Customer = null, EmployeeID = 2,Employee = null, OrderDate = new DateTime(2023, 04, 15), RequiredDate = new DateTime(2023, 02, 25), ShippedDate = new DateTime(2023, 02, 20), ShipVia = 2, Freight = 8.75, ShipName = "Order 2" },
           };
        private static List<OrderGetAllModel> GetTestOrderEntities =>
         new List<OrderGetAllModel>()
         {
                new OrderGetAllModel { OrderId = 1,EmployeeFullName = "sergii niverchuk",OrderDate = new DateTime(2023, 01, 10) },
                new OrderGetAllModel { OrderId = 2,EmployeeFullName = "illya zarech",OrderDate = new DateTime(2023, 04, 15) },
         };
        private static List<Customer> GetCustomers =>
        new List<Customer>()
        {
              new Customer {Id = 1,CompanyName = "first", ContactName = "first",ContactTitle = "first"},
              new Customer {Id = 2,CompanyName = "second", ContactName = "second",ContactTitle = "second"}
        };
        private static List<Employee> GetEmployees =>
         new List<Employee>()
         {
              new Employee {Id = 1,FirstName = "sergii", LastName = "niverchuk"},
              new Employee {Id = 2,FirstName = "illya", LastName = "zarech"}
         };
        private async Task CheckCustomersInfoIntoDb(int orderId, int expectedLength)
        {
            using (var test = _factory.Services.CreateScope())
            {
                var context = test.ServiceProvider.GetService<AppOrderContext>();
                context.Orders.Should().HaveCount(expectedLength);

                var dbOrder = await context.Orders.FindAsync(expectedLength);
                dbOrder.Should().NotBeNull();
                dbOrder.Freight.Should().Be(dbOrder.Freight);
            }
        }
        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
            _client.Dispose();
        }
    }
}
