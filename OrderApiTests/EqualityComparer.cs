﻿using OrderApi.Entities;
using OrderApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApiTests
{
    internal class OrderEqualityComparer : IEqualityComparer<Order>
    {
        public bool Equals([AllowNull] Order x, [AllowNull] Order y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && x.CustomerID == y.CustomerID
                && x.EmployeeID == y.EmployeeID
                && x.OrderDate == y.OrderDate
                && x.RequiredDate == y.RequiredDate
                && x.ShippedDate == y.ShippedDate
                && x.ShipVia == y.ShipVia
                && x.Freight == y.Freight
                && x.ShipName == y.ShipName;
        }

        public int GetHashCode([DisallowNull] Order obj)
        {
            return obj.GetHashCode();
        }
    }
    internal class OrderGetAllModelEqualityComparer : IEqualityComparer<OrderGetAllModel>
    {
        public bool Equals([AllowNull] OrderGetAllModel x, [AllowNull] OrderGetAllModel y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.OrderId == y.OrderId
                && x.EmployeeFullName == y.EmployeeFullName
                && x.OrderDate == y.OrderDate;
        }

        public int GetHashCode([DisallowNull] OrderGetAllModel obj)
        {
            return obj.GetHashCode();
        }
    }
}
