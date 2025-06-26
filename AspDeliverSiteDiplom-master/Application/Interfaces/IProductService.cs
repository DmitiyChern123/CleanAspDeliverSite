using DiplomApplication.Application.DTO;
using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Application.Interfaces;

public interface IProductService
{
    Task<List<Category>> LoadCategoryList();
    Task<HomeDto> LoadHomeData(HomeDto homeDto);
    Task AddProductInCart(string username, int productid, int propertyId);
    Task<EditProductDto> GetProductInfo(int productId);
    Task EditProduct(EditProductDto dto);
    Task<List<ProductType>> GetProductTypesById(int productid);
    Task ChangeVisabilityProductType(int productTypeId, bool isDeleted);
    Task AddProducts(List<Product> products);
    Task AddProductType(EditTypesDto dto);

}