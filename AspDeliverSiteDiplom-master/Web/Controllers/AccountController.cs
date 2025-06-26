using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using DiplomApplication.Application.DTO;
using DiplomApplication.Application.Interfaces;
using DiplomApplication.Domain.Entities;
using DiplomApplication.Infrastructure.Services;
using DiplomApplication.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DiplomApplication.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;
        
        public AccountController(IUserService iuserService, IMapper mapper,ILogger<AccountController> logger)
        {
            _mapper = mapper;
            _userService = iuserService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public static string HashString(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Преобразуем входную строку в байты и вычисляем хэш
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Конвертируем байты в шестнадцатеричную строку
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public async Task <IActionResult> Profile()
        {
            
            var username = User.Identity.Name;
            ProfileDto dto = await _userService.GetOrdersByUser(username);
            ProfileViewModel model = new ProfileViewModel()
            {
                user = dto.user,
                orders = dto.orders
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login (LoginViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                var findUser = await _userService.LoginUser((model.Login, model.Password));
                if (findUser ==null)
                {
                    ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
                    return View("Index", model);
                }
            try
            {
                _logger.LogInformation($"Пользователь {findUser.Login} успешно вошел в систему в систему");
                CreateClaims(findUser);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Ошибка входа " );
                return View();
            }
            
            }
            else
            {
                 return View("Index",model);
            }
        }
        
        public async Task <IActionResult> Registration(RegistrarionViewModel model)
        {
           return View();
        }

        private async Task CreateClaims(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Login)
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
        [HttpPost]
        public async Task<IActionResult> NewRegistration(RegistrarionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.RePassword)
                {
                    ModelState.AddModelError(string.Empty, "Пароли не совпадают");
                    return View("Registration", model);
                }
                var user = await _userService.RegisterUserAsync(_mapper.Map<NewUserDto>(model));
                CreateClaims(user); //Создание куки
            return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Registration",model);
            }
        }
    }
}
