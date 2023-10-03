using AutoMapper;
using Order.API.Application.Commands.CheckoutOrder;
using Order.API.Application.Commands.UpdateOrder;
using Order.API.Application.Queries;
using Order.API.Application.Queries.GetUserOrderHistory;
using Order.API.Dto;

namespace Order.API.Application.Mapper {
    public class MappingProfile : Profile {
        public MappingProfile () {
            CreateMap<Order.API.Models.Order, CheckoutOrderCommand> ().ReverseMap ();
            CreateMap<Order.API.Models.Order, UpdateOrderCommand> ().ReverseMap ();
            CreateMap<Order.API.Models.Order, OrderViewDto> ().ReverseMap ();
            CreateMap<GetUserOrderHistoryQuery, UserOrderHistoryRecord> ().ReverseMap ();
        }
    }
}