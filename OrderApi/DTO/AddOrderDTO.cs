namespace OrderApi.DTO
{
    public class AddOrderDTO
    {
        public int CustomerID { get; set; }

        public int EmployeeID { get; set; }

        public int ShipVia { get; set; }

        public double? Freight { get; set; }

        public string? ShipName { get; set; } = default!;
    }
}
