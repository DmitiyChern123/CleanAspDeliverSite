
using AutoMapper;
using DiplomApplication.Application.DTO;
using DiplomApplication.Application.Interfaces;
using DiplomApplication.Infrastructure.Services;
using DiplomApplication.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomApplication.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IOrderService _orderService;
        private const double FixedLatitude = 56.84495958999723;
        private const double FixedLongitude = 60.63911005278376;
        private readonly IMapper _mapper;
        //[56.84495958999723,60.63911005278376]
        public CartController(IOrderService iorderService,IMapper imapper)
        {
            _orderService = iorderService;
            _mapper = imapper;
        }
        [Authorize(Roles = "user")]
        public async Task<ActionResult> NewOrder(CartViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                CartDto dto = _mapper.Map<CartDto>(model);
                dto.username = User.Identity.Name;
                var d  = await _orderService.CreateNewOrder(dto);
                
                return View("successOrder");
            }
            return RedirectToAction("СreateOrder", model);
           
        }
        [Authorize(Roles = "user")]
        public async Task< ActionResult> successOrder ()
        {
            return View();
        }
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Радиус Земли в километрах
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c; // Расстояние в километрах
            return distance * 1000; // Переводим в метры
        }
        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
        // GET: Korzinas
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            CartDto dto = await _orderService.GetUserCartInfo(username);
            CartViewModel model = new CartViewModel()
            {
                order_sum = dto.order_sum,
                products = dto.products,
                korzinas = dto.korzinas
            };
            return View(model);
        }

        public async Task<IActionResult> СreateOrder( CartViewModel cartmodel)
        { 
            var username = User.Identity.Name;
            CartDto dto = await _orderService.GetUserCartInfo(username);
            CartViewModel model = new CartViewModel()
            {
                order_sum = dto.order_sum,
                products = dto.products,
                korzinas = dto.korzinas,
                cur_bonuses = dto.cur_bonuses,
                Is_need_devices = dto.Is_need_devices
            };
            return View(model);
        }
     
        [HttpPost]
        public async Task<ActionResult> UpdateQuantity(int id, int newCount)
        {
            _orderService.UpdateQuantity(id, newCount);
            return RedirectToAction("Index");
        }
        
        public async Task<ActionResult> MinusProduct(int korid,CartViewModel model)
        { 
            var username = User.Identity.Name;
            _orderService.RemoveCart(korid, username);
            return RedirectToAction("Index");
        }
    }
}
