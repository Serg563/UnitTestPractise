using Moq;
using NUnit.Framework;
using OrderApi.Entities;
using OrderApi.Models;
using OrderApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using OrderApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using OrderApi.DTO;

namespace OrderApiTests.Controller
{
    [TestFixture]
    internal class OrderControllerTest
    {

        [Test]
        public async Task OrderController_GetAllOrder_ReturnAll()
        {
            var mockOrder = new Mock<IOrderService>();

            mockOrder
               .Setup(x => x.GetAllOrders())
               .ReturnsAsync(GetTestOrderGetAllModelEntities.AsEnumerable());

            var controller = new OrderServiceController(mockOrder.Object);
            var actualResult = await controller.GetAllOrder();

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult.Result, Is.Not.Null);
            Assert.That(actualResult.Result, Is.InstanceOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)actualResult.Result!).Value, Is.EqualTo(GetTestOrderGetAllModelEntities).Using(new OrderGetAllModelEqualityComparer()));
            mockOrder.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task OrderController_GetOrderById_ReturnFirst(int id)
        {
            var mockOrder = new Mock<IOrderService>();

            mockOrder
               .Setup(x => x.GetOrderById(id))
               .ReturnsAsync(GetTestOrderModels.FirstOrDefault(x => x.Id == id));

            var controller = new OrderServiceController(mockOrder.Object);
            var actualResult = await controller.GetOrderById(id);

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult, Is.InstanceOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)actualResult!).Value, Is.EqualTo(GetTestOrderModels.FirstOrDefault(x=>x.Id == id)).Using(new OrderEqualityComparer()));
            mockOrder.VerifyAll();
        }

        [Test]
        public async Task OrderController_AddOrderDTO()
        {
            var mockOrder = new Mock<IOrderService>();

            mockOrder.Setup(x => x.AddOrder(It.IsAny<AddOrderDTO>()));

            var controller = new OrderServiceController(mockOrder.Object);
            var addOrderDTO = new AddOrderDTO()
            {
                CustomerID = 1,
                EmployeeID = 1,
                ShipVia = 1,
                ShipName = "Test",
                Freight = 15
            };
            var actualResult = await controller.AddOrder(addOrderDTO);

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult, Is.InstanceOf(typeof(OkResult)));
            mockOrder.Verify(x => x.AddOrder(It.Is<AddOrderDTO>(x => x.ShipName == addOrderDTO.ShipName)), Times.Once);

            mockOrder.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public async Task OrderController_DeleteOrder(int id)
        {
            var mockOrder = new Mock<IOrderService>();

            mockOrder.Setup(x => x.DeleteOrder(It.Is<int>(x => x == id))).Verifiable();

            var controller = new OrderServiceController(mockOrder.Object);
          
            var actualResult = await controller.DeleteOrder(id);

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult, Is.InstanceOf(typeof(NoContentResult)));

            mockOrder.VerifyAll();
        }

        [TestCase(3)]
        public async Task OrderController_DeleteOrder_OrderIsNotExist_ReturnNotFound(int id)
        {
            var mockOrder = new Mock<IOrderService>();

            mockOrder.Setup(x => x.DeleteOrder(It.Is<int>(x => x == id)))
                .ThrowsAsync(new IndexOutOfRangeException())
                .Verifiable();

            var controller = new OrderServiceController(mockOrder.Object);

            Assert.ThrowsAsync<IndexOutOfRangeException>(async () => await controller.DeleteOrder(id));

            mockOrder.VerifyAll();
        }

        [TestCase(2)]
        public async Task OrderController_UpdateOrder_ReturnNoContext(int id)
        {
            var mockOrder = new Mock<IOrderService>();

            mockOrder.Setup(x => x.UpdateOrder(It.IsAny<Order>()));

            var controller = new OrderServiceController(mockOrder.Object);
            var order = GetTestOrderModels.First();

            var actualResult = await controller.UpdateOrder(order);

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult, Is.InstanceOf(typeof(NoContentResult)));

            mockOrder.VerifyAll();
        }
        private static List<Order> GetTestOrderModels =>
         new List<Order>()
         {
                new Order { Id = 1, CustomerID = 1,Customer = GetCustomers[0], EmployeeID = 1,Employee = GetEmployees[0], OrderDate = new DateTime(2023, 01, 10), RequiredDate = new DateTime(2023, 01, 20), ShippedDate = new DateTime(2023, 01, 15), ShipVia = 1, Freight = 12.5, ShipName = "Order 1" },
                new Order { Id = 2, CustomerID = 2,Customer = GetCustomers[1], EmployeeID = 2,Employee = GetEmployees[1], OrderDate = new DateTime(2023, 04, 15), RequiredDate = new DateTime(2023, 02, 25), ShippedDate = new DateTime(2023, 02, 20), ShipVia = 2, Freight = 8.75, ShipName = "Order 2" },
         };
        private static List<OrderGetAllModel> GetTestOrderGetAllModelEntities =>
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
    }
}
