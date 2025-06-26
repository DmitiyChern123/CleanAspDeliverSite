using DiplomApplication.Application.DTO;
using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Application.Interfaces;

public interface IUserService
{
    Task<User> GetUserByIdAsync(int id);
    Task<User> LoginUser((string, string) LoginDTO);
    Task<User> RegisterUserAsync(NewUserDto newUserDto); 
    Task DeleteUserAsync(int id);
    Task<ProfileDto> GetOrdersByUser(string username);

}