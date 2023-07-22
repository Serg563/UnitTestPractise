namespace OrderApi.Models
{
    public class OrderGetAllModel
    {
        public int OrderId { get; set;}

        public string? EmployeeFullName { get; set;}

        public DateTime OrderDate { get; set;}
    }
}
