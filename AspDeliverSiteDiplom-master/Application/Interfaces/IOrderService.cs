using DiplomApplication.Application.DTO;

namespace DiplomApplication.Application.Interfaces;

public interface IOrderService
{
    Task UpdateQuantity(int id, int newCount);
    Task<CartDto> GetUserCartInfo(string username);
    
    Task<CartDto> GetPreOrderInfo(CartDto? dt, string username);
    Task RemoveCart(int korid, string username);
    Task<CartDto> CreateNewOrder(CartDto dto);
    Task<OrdersDTO> OrderEdit();
    Task GetCourier(int orderid, int courierid);
    Task changestatus(int orderid, int statusid);
    Task CreateTestOrder();
}