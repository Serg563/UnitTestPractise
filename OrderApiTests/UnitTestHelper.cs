using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderApi;
using OrderApi.Data;
using OrderApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApiTests
{
    internal class UnitTestHelper
    {
        public static DbContextOptions<AppOrderContext> GetUnitTestDbOptions()
        {
            var options = new DbContextOptionsBuilder<AppOrderContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new AppOrderContext(options))
            {
                SeedData(context);
            }

            return options;
        }
        public static void SeedData(AppOrderContext context)
        {
            context.Categories.AddRange(
                new Category { CategoryID = 1, CategoryName = "Category 1", Description = "Description for Category 1" },
                new Category { CategoryID = 2, CategoryName = "Category 2", Description = "Description for Category 2" });
            context.Customers.AddRange(
                new Customer { Id = 1, CompanyName = "Company A", ContactName = "John Doe", ContactTitle = "Manager" },
                new Customer { Id = 2, CompanyName = "Company B", ContactName = "Jane Smith", ContactTitle = "CEO" });
            context.Employees.AddRange(
                new Employee { Id = 1, FirstName = "sergii", LastName = "niverchuk", BirthDate = new DateTime(1990, 5, 4), Title = "Jedi", Country = "Tatooine", City = "Mos Eisley" },
                new Employee { Id = 2, FirstName = "illya", LastName = "zarech", BirthDate = new DateTime(1992, 7, 12), Title = "Princess", Country = "Alderaan", City = "Aldera" });
            context.Orders.AddRange(
                new Order { Id = 1, CustomerID = 1, EmployeeID = 1, OrderDate = new DateTime(2023, 01, 10), RequiredDate = new DateTime(2023, 01, 20), ShippedDate = new DateTime(2023, 01, 15), ShipVia = 1, Freight = 12.5, ShipName = "Order 1" },
                new Order { Id = 2, CustomerID = 2, EmployeeID = 2, OrderDate = new DateTime(2023, 04, 15), RequiredDate = new DateTime(2023, 02, 25), ShippedDate = new DateTime(2023, 02, 20), ShipVia = 2, Freight = 8.75, ShipName = "Order 2" });
            context.OrderDetails.AddRange(
                new OrderDetail { OrderDetailsID = 1, OrderID = 1, ProductID = 1, UnitPrice = 25.5, Quantity = 5, Discount = 0.1 },
                new OrderDetail { OrderDetailsID = 2, OrderID = 1, ProductID = 2, UnitPrice = 15.0, Quantity = 2, Discount = 0.05 },
                new OrderDetail { OrderDetailsID = 3, OrderID = 2, ProductID = 2, UnitPrice = 15.0, Quantity = 10, Discount = 0.2 });
            context.Shippers.AddRange(
                new Shipper { Id = 1, CompanyName = "Shipper A", Phone = "123-456-7890" },
                new Shipper { Id = 2, CompanyName = "Shipper B", Phone = "987-654-3210" });
            context.Suppliers.AddRange(
                new Supplier { SipplierID = 1, SupplierName = "Supplier X", ContactName = "John Supplier", ContactTitle = "Manager", Address = "123 Supplier Street", City = "Supplier City", Region = "Supplier Region", PostalCode = "12345", Country = "Supplier Country", Phone = "111-222-3333", Fax = "444-555-6666", HomePage = "www.supplierx.com" },
                new Supplier { SipplierID = 2, SupplierName = "Supplier Y", ContactName = "Jane Supplier", ContactTitle = "Sales Representative", Address = "456 Supplier Avenue", City = "Supplierville", Region = "Supplier State", PostalCode = "54321", Country = "Supplier Country", Phone = "777-888-9999", Fax = "000-111-2222", HomePage = "www.suppliery.com" });
            context.Products.AddRange(
                new Product { Id = 1, ProductName = "Product 1", SupplierID = 1, CategoryId = 1 },
                new Product { Id = 2, ProductName = "Product 2", SupplierID = 2, CategoryId = 2 });
            context.SaveChanges();
        }
        public static IMapper CreateMapperProfile()
        {
            var myProfile = new AutoMapperProfiler();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));

            return new Mapper(configuration);
        }
    }
}
