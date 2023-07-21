using AutoMapper;
using OrderApi.DTO;
using OrderApi.Entities;

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
        }
    }
}
