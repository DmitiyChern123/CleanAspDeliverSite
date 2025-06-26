using AutoMapper;
using DiplomApplication.Application.DTO;
using DiplomApplication.Web.Models;

namespace DiplomApplication.MapperProfile;

public class AutoMapperProfile : Profile 
{
    public AutoMapperProfile() 
    {
        // Маппинг из UserViewModel в UserDto
        CreateMap<RegistrarionViewModel, NewUserDto>();
        CreateMap<HomeModel, HomeDto>();
        CreateMap<CartViewModel, CartDto>();
        CreateMap<EditProductViewModel, EditProductDto>();
        CreateMap<OrdersDTO, OrdersViewModel>();
        // Маппинг из UserDto в UserEntity
        //CreateMap<UserDto, UserEntity>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Игнорируем поле
    }
}