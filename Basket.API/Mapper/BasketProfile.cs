using AutoMapper;
using Basket.API.Models;
using Basket.API.Models.ShoppingCartAggregate;
using EventBus.Messages.Events;

namespace Basket.API.Mapper {
    public class BasketProfile : Profile {
        public BasketProfile () {
            CreateMap<BasketCheckout, BasketCheckoutEvent> ().ReverseMap ();
            CreateMap<ShoppingCartItem, ShoppingItem>().ReverseMap();
        }
    }
}