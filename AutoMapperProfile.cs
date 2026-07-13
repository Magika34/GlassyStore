using AutoMapper;
using GlassyStore.Models;
using GlassyStore.ViewModels;

namespace GlassyStore.Mapping 
{ 
    public class AutoMapperProfile : Profile 
    { 
        public AutoMapperProfile() {
            CreateMap<Product, ProductViewModel>(); 
            CreateMap<ProductViewModel, Product>(); 
            CreateMap<OrderItem, CheckoutViewModel>(); 
            CreateMap<CheckoutViewModel, OrderItem>(); }
    } 
}