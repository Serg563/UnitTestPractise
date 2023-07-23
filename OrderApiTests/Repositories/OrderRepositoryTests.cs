using AutoMapper;
using NUnit.Framework;
using OrderApi.Data;
using OrderApi.DTO;
using OrderApi.Entities;
using OrderApiTests;

namespace OrderApi.Repositories.Tests
{
    [TestFixture]
    public class OrderRepositoryTests
    {
        [Test]
        public async Task GetAllOrdersTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfiler>());
            var mapper = config.CreateMapper();

            using var context = new AppOrderContext(UnitTestHelper.GetUnitTestDbOptions());
            var orderRepository = new OrderRepository(context, mapper);
            var orders = await orderRepository.GetAllOrders();
            Assert.That(orders, Is.EqualTo(ExpectedOrders).Using(new OrderEqualityComparer()), message: "GetAllOrders method works incorrect");
        }
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetOrderByIdTest(int id)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfiler>());
            var mapper = config.CreateMapper();

            using var context = new AppOrderContext(UnitTestHelper.GetUnitTestDbOptions());
            var orderRepository = new OrderRepository(context, mapper);
            var order = await orderRepository.GetOrderById(id);
            var expected = ExpectedOrders.FirstOrDefault(x => x.Id == id);
            Assert.That(order, Is.EqualTo(expected).Using(new OrderEqualityComparer()), message: "GetOrderById method works incorrect");
        }
        [Test]
        public async Task DeleteOrderTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfiler>());
            var mapper = config.CreateMapper();

            using var context = new AppOrderContext(UnitTestHelper.GetUnitTestDbOptions());
            var orderRepository = new OrderRepository(context, mapper);
            var deleted = await orderRepository.DeleteOrder(1);
            Assert.That(context.Orders.Count(), Is.EqualTo(1), message: "DeleteOrderTest method works incorrect");
        }
        [Test]
        public async Task AddOrderTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfiler>());
            var mapper = config.CreateMapper();

            using var context = new AppOrderContext(UnitTestHelper.GetUnitTestDbOptions());
            var orderRepository = new OrderRepository(context, mapper);
            AddOrderDTO order = new AddOrderDTO() { Freight = 5 };
            orderRepository.AddOrder(order);
            Assert.That(context.Orders.Count(), Is.EqualTo(3), message: "AddOrderTest method works incorrect");
        }
        [Test]
        public async Task UpdateOrderTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfiler>());
            var mapper = config.CreateMapper();

            using var context = new AppOrderContext(UnitTestHelper.GetUnitTestDbOptions());
            var orderRepository = new OrderRepository(context, mapper);
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
            orderRepository.UpdateOrder(order);
            await context.SaveChangesAsync();
            Assert.That(order, Is.EqualTo(new Order
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
            }).Using(new OrderEqualityComparer()), message: "UpdateOrderTest method works incorrect");
        }
        private static IEnumerable<Order> ExpectedOrders =>
           new[]
           {
                new Order { Id = 1, CustomerID = 1, EmployeeID = 1, OrderDate = new DateTime(2023, 01, 10), RequiredDate = new DateTime(2023, 01, 20), ShippedDate = new DateTime(2023, 01, 15), ShipVia = 1, Freight = 12.5, ShipName = "Order 1" },
                new Order { Id = 2, CustomerID = 2, EmployeeID = 2, OrderDate = new DateTime(2023, 04, 15), RequiredDate = new DateTime(2023, 02, 25), ShippedDate = new DateTime(2023, 02, 20), ShipVia = 2, Freight = 8.75, ShipName = "Order 2" },
           };
    }
}