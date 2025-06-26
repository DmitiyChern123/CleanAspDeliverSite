using System.Security.Cryptography;
using System.Text;
using DiplomApplication.Application.DTO;
using DiplomApplication.Application.Interfaces;
using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService( IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> GetUserByIdAsync(int id)
        => await _userRepository.GetByIdAsync(id);

    public async Task<User> LoginUser((string,string) LoginDTO )
    {
        return await _userRepository.GetUserFromLoginAndPasswordHashAsync(LoginDTO.Item1, HashString(LoginDTO.Item2));
    }
    public static string HashString(string input)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Преобразуем входную строку в байты и вычисляем хэш
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Конвертируем байты 
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
    public async Task<User> RegisterUserAsync( NewUserDto newUserDto)
    {
        if (newUserDto.Password != newUserDto.RePassword)
            throw new ArgumentException("Пароли не совпадают");
        User user = new User()
        {   
            Name = newUserDto.Name,
            Login = newUserDto.Login,
            Password = HashString(newUserDto.Password),
            Role = "user",
            BonusPoints = 0
        };
        
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(); // Фиксация транзакции
        return user;
    }
    public async Task DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user != null)
        {
            await _userRepository.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<ProfileDto> GetOrdersByUser(string username)
    {
        User user = await _userRepository.GetByLoginAsync(username);
        ProfileDto dto = new ProfileDto()
        {
            user = user,
            orders = await _userRepository.GetOrdersByUserAsync(user.Id)
        };
        return dto;
    }
}