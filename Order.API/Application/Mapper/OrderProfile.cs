using AutoMapper;
using Order.API.Application.Commands.CheckoutOrder;
using Order.API.Application.Commands.UpdateOrder;

namespace Order.API.Application.Mapper {
    public class MappingProfile : Profile {
        public MappingProfile () {
            CreateMap<Order.API.Models.Order, CheckoutOrderCommand> ().ReverseMap ();
            CreateMap<Order.API.Models.Order, UpdateOrderCommand> ().ReverseMap ();
        }
    }
}