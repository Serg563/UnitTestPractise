using AutoMapper;
using OrderApi.DTO;
using OrderApi.Entities;
using OrderApi.Models;

namespace OrderApi
{
    public class AutoMapperProfiler : Profile
    {
        public AutoMapperProfiler()
        {
            CreateMap<AddOrderDTO, Order>()
                .ForMember(o => o.CustomerID, ao => ao.MapFrom(x => x.CustomerID))
                .ForMember(o => o.EmployeeID, ao => ao.MapFrom(x => x.EmployeeID))
                .ForMember(o => o.ShipVia, ao => ao.MapFrom(x => x.ShipVia))
                .ForMember(o => o.Freight, ao => ao.MapFrom(x => x.Freight))
                .ForMember(o => o.ShipName, ao => ao.MapFrom(x => x.ShipName)).ReverseMap();

            CreateMap<Order, OrderGetAllModel>()
                .ForMember(ogm => ogm.OrderId, o => o.MapFrom(x => x.Id))
                .ForMember(ogm => ogm.EmployeeFullName, o => o.MapFrom(x => $"{x.Employee.FirstName} {x.Employee.LastName}"))
                .ForMember(ogm => ogm.OrderDate, o => o.MapFrom(x => x.OrderDate)).ReverseMap();
                
                

        }
    }
}
