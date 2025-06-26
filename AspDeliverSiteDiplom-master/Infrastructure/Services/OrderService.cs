using DiplomApplication.Application.DTO;
using DiplomApplication.Application.Interfaces;
using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IProductRepository productRepository, IUserRepository userRepository, IOrderRepository orderRepository ,IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CartDto> GetUserCartInfo(string username)
    {
        var user = await _userRepository.GetByLoginAsync(username);
        var list = await _productRepository.GetCartsFromUserIdAsync(user.Id);
        var producttypes = await _productRepository.GetProductTypesAsync();
        int sum = 0;
        foreach (var d in list)
        {
            sum += d.ProductType.Price * d.Count;
        }

        CartDto dto = new CartDto()
        {
            korzinas = list,
            order_sum = sum,
            products = producttypes,
            cur_bonuses = (int)user.BonusPoints,
            Is_need_devices = true
            
        };
        return dto;

    }

    public async Task UpdateQuantity(int id, int newCount)
    {
        if (newCount>0)
        {
            
        _productRepository.UpdateQuantityCart(id, newCount);
        _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task RemoveCart(int korid, string username)
    {
        if (korid != null && korid !=0)
        {
            User user = await _userRepository.GetByLoginAsync(username);
            var list = await _productRepository.GetCartsFromUserIdAsync(user.Id);
            var cart = list.FirstOrDefault(d => d.Id == korid);
            if (cart != null)
            {
                _productRepository.DeleteAsync(cart);
                _unitOfWork.SaveChangesAsync();
            }
            
        }
    }
    public async Task<CartDto> GetPreOrderInfo(CartDto dto,string username)
    {
        User user = await _userRepository.GetByLoginAsync(username);
        dto.korzinas = await _productRepository.GetCartsFromUserIdAsync(user.Id);
        return dto;
    }

    public async Task<CartDto> CreateNewOrder(CartDto dto)
    {
        Order order = new Order()
        {
            Adress = dto.Address,
            Phone = dto.PhoneNumber.ToString(),
            PayType = dto.PaymentType,
            Date = DateTime.Now,
            StatusId = 1,
            IsNeedDevices = dto.Is_need_devices
        };
        var user = await _userRepository.GetByLoginAsync(dto.username);
        user.BonusPoints -= dto.rem_bonuses;
        var carts = await _productRepository.GetCartsFromUserIdAsync(user.Id);
        int totalsum = 0;
        foreach (var item in  carts)
        {
            totalsum += item.ProductType.Price * item.Count;
                    
        }
        //скидка 
        if (user.Orders.Count == 0)//_context.Orders.Where(d=>d.UserId == user.Id).Count() ==0)
        {
            totalsum = Convert.ToInt32(totalsum * 0.80);
        }
        totalsum -= dto.rem_bonuses;
        user.BonusPoints = user.BonusPoints+Convert.ToInt32( totalsum * ( Convert.ToDouble(5) / 100)) ;
        order.UserId = user.Id;
        order.Sum = totalsum;
        foreach (var item in carts)
        {
            var newinorder = new CartInOrder()
            {
                Order = order,
                ProductId = item.ProductTypeId,
                Count = item.Count
            };
            _orderRepository.AddAsync(newinorder);
        }
        foreach (var item in carts)
        {
            _productRepository.DeleteAsync(item);
        }

        _orderRepository.AddAsync(order);
        _unitOfWork.SaveChangesAsync();
        
        
        return dto;
    }

    public async Task<OrdersDTO> OrderEdit()
    {
        var dto = new OrdersDTO()
        {
            Orders = await _orderRepository.GetFullOrdersInfoAsync(),
            statuses = await _orderRepository.GetStatusOrdersAsync(),
            courses = await _orderRepository.GetCouriersAsync()
        };
        DateTime now = DateTime.Now;
        DateTime tenMinutesAgo = now.AddMinutes(-10);

        // Фильтрация курьеров, которые не имели заказов за последние 10 минут
        dto.courses = dto.courses
            .Where(c => c.Orders == null || !c.Orders.Any(o => o.Date >= tenMinutesAgo && o.Date <= now))
            .ToList();
        return dto;

    }

    public async Task GetCourier(int orderid, int courierid)
    {
        var orderlist = await _orderRepository.GetFullOrdersInfoAsync();
        var order = orderlist.FirstOrDefault(d=>d.Id == orderid);
        if (orderid != 0)
        {
            if (courierid != 0)
            {
                order.CourierId = courierid;
                if (order != null)
                {
                    _orderRepository.UpdateAsync(order);
                }
            }
            else
            {
                order.CourierId = null;
            }
        }

        _unitOfWork.SaveChangesAsync();

    }

    public async Task changestatus(int orderid, int statusid)
    {
        if (orderid != 0)
        {
            List<Order> orders = await _orderRepository.GetFullOrdersInfoAsync();
            var order = orders .FirstOrDefault(d => d.Id == orderid);
            if (order!=null)
            {
                order.StatusId = statusid;
                _orderRepository.UpdateAsync(order);
                _unitOfWork.SaveChangesAsync();
            }
        }
    }

    public async Task CreateTestOrder()
    {
        Order order = new Order
        {
            UserId = 3,
            Adress = "test value",
            Phone = "+7 900 123 4567",
            StatusId = 1,
            Date = DateTime.Now,
            IsNeedDevices = true,
            PayType = "none"
                
        };
        _orderRepository.AddAsync(order);
        _unitOfWork.SaveChangesAsync();
    }
}