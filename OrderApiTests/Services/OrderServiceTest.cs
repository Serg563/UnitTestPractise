using Moq;
using NUnit.Framework;
using OrderApi.Data;
using OrderApi.Entities;
using OrderApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using OrderApi.Models;
using OrderApi.DTO;
using Castle.Core.Resource;

namespace OrderApiTests.Services
{
    [TestFixture]
    internal class OrderServiceTest
    {
        [Test]
        public async Task GetAllOrderTest()
        {
            //arrange
            var expected = GetTestOrderEntities;
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockUnitOfWork
                .Setup(x => x.OrderRepository.GetAllOrders())
                .ReturnsAsync(GetTestOrderModels.AsEnumerable());

            var orderService = new OrderService(mockUnitOfWork.Object, UnitTestHelper.CreateMapperProfile());

            //act
            var actual = await orderService.GetAllOrders();

            //assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase(1)]
        [TestCase(2)]
        public async Task OrderService_GetOrderById_ReturnSingle(int id)
        {
            var expected = GetTestOrderModels.FirstOrDefault(x => x.Id == id);
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockUnitOfWork.Setup(x => x.OrderRepository.GetOrderById(id))
                .ReturnsAsync(GetTestOrderModels.FirstOrDefault(x => x.Id == id));

            var orderService = new OrderService(mockUnitOfWork.Object, UnitTestHelper.CreateMapperProfile());

            var order = await orderService.GetOrderById(id);

            Assert.That(order, Is.EqualTo(expected).Using(new OrderEqualityComparer()));   
        }

        [TestCase(1)]
        [TestCase(2)]
        public async Task OrderService_DeleteOrder_DeletedCustomer(int id)
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.OrderRepository.DeleteOrder(It.IsAny<int>()));
            var orderService = new OrderService(mockUnitOfWork.Object, UnitTestHelper.CreateMapperProfile());

            await orderService.DeleteOrder(id);

            mockUnitOfWork.Verify(x => x.OrderRepository.DeleteOrder(id), Times.Once());
            mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once());
        }

        [Test]
        public async Task OrderService_UpdateOrder_UpdatedOrder()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.OrderRepository.UpdateOrder(It.IsAny<Order>()));
            var orderService = new OrderService(mockUnitOfWork.Object, UnitTestHelper.CreateMapperProfile());
            var order = GetTestOrderModels.First();

            await orderService.UpdateOrder(order);

            mockUnitOfWork.Verify(x => x.OrderRepository.UpdateOrder(It.Is<Order>(x => x.Id == order.Id
            && x.OrderDate == order.OrderDate)), Times.Once());
            mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once());
        }

        [Test]
        public async Task OrderService_AddOrder_AddedOrder()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.OrderRepository.AddOrder(It.IsAny<AddOrderDTO>()));
            var orderService = new OrderService(mockUnitOfWork.Object, UnitTestHelper.CreateMapperProfile());

            var addOrderDTO = new AddOrderDTO()
            {
                CustomerID = 1,
                EmployeeID = 1,
                ShipVia = 1,
                ShipName= "Test",
                Freight = 15
            };

            await orderService.AddOrder(addOrderDTO);

            mockUnitOfWork.Verify(x => x.OrderRepository.AddOrder(It.Is<AddOrderDTO>(x => x.ShipName == addOrderDTO.ShipName)), Times.Once);
            mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once());
        }

        private static List<Order> GetTestOrderModels =>
           new List<Order>()
           {
                new Order { Id = 1, CustomerID = 1,Customer = GetCustomers[0], EmployeeID = 1,Employee = GetEmployees[0], OrderDate = new DateTime(2023, 01, 10), RequiredDate = new DateTime(2023, 01, 20), ShippedDate = new DateTime(2023, 01, 15), ShipVia = 1, Freight = 12.5, ShipName = "Order 1" },
                new Order { Id = 2, CustomerID = 2,Customer = GetCustomers[1], EmployeeID = 2,Employee = GetEmployees[1], OrderDate = new DateTime(2023, 04, 15), RequiredDate = new DateTime(2023, 02, 25), ShippedDate = new DateTime(2023, 02, 20), ShipVia = 2, Freight = 8.75, ShipName = "Order 2" },
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
    }
}
