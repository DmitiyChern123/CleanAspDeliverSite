using System.Diagnostics;
using System.Reflection;
using AutoMapper;
using DiplomApplication.Application.DTO;
using DiplomApplication.Application.Interfaces;
using DiplomApplication.Domain.Entities;
using DiplomApplication.Infrastructure.Services;
using DiplomApplication.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiplomApplication.Web.Controllers
{
    public class HomeController : Controller
    {
        private static List<Product> _pendingProducts = new List<Product>();
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ILogger<HomeController> logger , IWebHostEnvironment webHostEnvironment,IProductService iproductService, IOrderService iorderService,IMapper mapper )//(ILogger<HomeController> logger, DiplomContext context, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _productService = iproductService;
            _orderService = iorderService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }
        
        public async Task<ActionResult> Loadfiles ()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Preview()
        {
            var file = Request.Form.Files[0];
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                var jsonContent = await stream.ReadToEndAsync();
                _pendingProducts = JsonConvert.DeserializeObject<List<Product>>(jsonContent);
            }
            return Json(_pendingProducts);
        }
        
        [HttpPost]
        public async Task< IActionResult> Confirm()
        {
            if (_pendingProducts != null && _pendingProducts.Count > 0)
            {

                _productService.AddProducts(_pendingProducts);   
                _pendingProducts.Clear();
            }
            return Ok();
        }
        
        public async Task<ActionResult> AddProductType(EditTypesViewModel model)
        {
            if (model.productid != 0)
            {
               EditTypesDto dto = new EditTypesDto()
              {
                newtype = model.newtype,
                productid = model.productid
              };
             _productService.AddProductType(dto);
             return RedirectToAction("EditTypes", "Home", new { productid = model.productid });
            }
            else
            {
                return RedirectToAction("EditTypes");
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> UpdateProductType(int productId, int productTypeId, bool isDeleted)
        {
            _productService.ChangeVisabilityProductType(productTypeId, isDeleted);
            return RedirectToAction("EditTypes", "Home", new { productid = productId });
        }
        
        private async Task<List<JObject>> ReadLogEntries(DateTime date)
        {
            
            List<JObject> logEntries = new List<JObject>();
            var path = $"Logs/log-{date:yyyyMMdd}.json";
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var streamReader = new StreamReader(fileStream))
                {
                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            try
                            {
                                var log = JObject.Parse(line);
                                logEntries.Add(log);
                            }
                            catch (Exception ex)
                            {
                                // логируем или пропускаем некорректную строку
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading log file: " + ex.Message);
            }
            return logEntries;
        }
        [Authorize(Roles = "manager")]
        public async Task <ActionResult> OrderEdit(int statusid)
        {
            var dto = await _orderService.OrderEdit();
           var model = _mapper.Map<OrdersViewModel>(dto);
            return View(model);
            return null;
        }
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> EditTypes(int productid)
        {
            EditTypesViewModel editTypesViewModel = new EditTypesViewModel()
            {
                types = await _productService.GetProductTypesById(productid),
                productid = productid
            };
            return View("EditTypes", editTypesViewModel);
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> EndEditing(EditProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                EditProductDto dto = _mapper.Map<EditProductDto>(model);
                _productService.EditProduct(dto);
                return RedirectToAction("Index", model);
            }
            else
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                }
                //model.categories = _context.Categories.ToList();
                return View("CreateOrEditProduct", model);
            }
        }
        public static bool AreAllPropertiesNull(object obj)
        {
            if (obj == null)
            {
                return true;
            }
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj);
                if (value != null)
                {
                    return false;
                }
            }

            return true;
        }
        
        public ActionResult TestOrder()
        {
            _orderService.CreateTestOrder();
            return RedirectToAction("OrderEdit");
        }
        
        public async Task< ActionResult> changestatus(int orderid, int statusid)
        {
            _orderService.changestatus(orderid, statusid);
            return RedirectToAction("OrderEdit");
        }
        
        public async Task<ActionResult> getCourirer(int orderid,int courierid)
        {
            _orderService.GetCourier(orderid,courierid);
            return RedirectToAction("OrderEdit");
        }
        
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateOrEditProduct(int productid)
        {
            EditProductDto dto = await _productService.GetProductInfo(productid);
            EditProductViewModel editProductViewModel = new EditProductViewModel()
            {
                categories = dto.categories,
                product = dto.product,
                selcat = dto.selcat,
                types = dto.types,
                ImgFile = dto.ImgFile
            };
            return View("CreateOrEditProduct", editProductViewModel);
        }
        
        public async Task<ActionResult> Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
            return RedirectToAction("Index", "Home");
        }
        
        public async Task<IActionResult> Index(int? categoryId, string sortOrder, string searchQuery)
        {
            HomeDto homeDto = new HomeDto()
            {
                SearchQuery = searchQuery,
                SelectedCategoryId = categoryId,
                SortOrder = sortOrder,
                UserName = User.Identity.Name,
            };
            homeDto = await _productService.LoadHomeData(homeDto);
            ViewBag.CartInfo = homeDto.CartInfo.AsEnumerable();
            ViewBag.SearchQuery = searchQuery;
            HomeModel viewModel = new HomeModel()
            {
                Categories = homeDto.Categories,
                Products = homeDto.Products.ToList(),
                SelectedCategoryId = homeDto.SelectedCategoryId,
                SortOrder = homeDto.SortOrder,
                SearchQuery = homeDto.SearchQuery,
                hasinkorzina = homeDto.hasinkorzina
            };
            return View(viewModel);
        }
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddProduct(int productid, int propertyId)
        {
            var username = User.Identity.Name;
            _productService.AddProductInCart(username, productid, propertyId);
            return RedirectToAction("Index", "Home");
            
        }

        public async Task< IActionResult> CheckLogin()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Json(new { isAuthenticated = false, redirectUrl = "/Account/Index" });
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> LoggerPage(DateTime? date, string errorType)
        {
            LogsViewModel logsViewModel = new LogsViewModel();
            List<JObject> logEntries = new List<JObject>();
            try
            {
                if (date != null)
                {
                     logEntries = await ReadLogEntries((DateTime)date);
                     if (errorType != null && errorType != "")
                     {
                     logEntries = logEntries.Where(log => log["Level"]?.ToString() == errorType).ToList();
                     }
                }
                else
                {
                    date = DateTime.Now; 
                    logEntries = await ReadLogEntries((DateTime)date);
                    if (errorType != null && errorType != "")
                    {
                        logEntries = logEntries.Where(log => log["Level"]?.ToString() == errorType).ToList();
                    }
                }

                if (logEntries != null)
                {
                    logEntries = logEntries.Where(log => log["Timestamp"].Value<DateTime>().Date == date).ToList();
                }
                if (!string.IsNullOrEmpty(errorType))
                {
                    logEntries = logEntries.Where(log => log["Level"].Value<string>() == errorType).ToList();
                }
                logsViewModel.SelectedDate = (DateTime)date;
                logsViewModel.Logs = logEntries;
            }
            catch (Exception ex)
            {
                logsViewModel.ErrorMessage = "Error reading log file: " + ex.Message;
            }
            return View(logsViewModel);
        }
    }
}
