using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Category>> GetCatalogAsync();
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    
    
    Task AddAsync(Cart cart);
    Task UpdateAsync(Cart cart);
    Task DeleteAsync(Cart cart);

    Task AddAsync(ProductType productType);
    Task UpdateAsync(ProductType productType);
    Task DeleteAsync(ProductType productType);
    Task<ProductType> GetProductTypeByIdAsync(int id);

    Task<List<Cart>> GetCartsFromUserIdAsync(int userid);
    Task<List<Category>> GetCategoriesAsync();
    Task<List<Product>> GetProductsAsync();
    Task<bool> CheckCartAsync(int ProductTypeId, int UserID);
    Task CartPlusCountAsync(int ProductTypeId, int UserID, int count);
    Task<List<ProductType>> GetProductTypesAsync();
    Task UpdateQuantityCart(int id, int newCount);
    Task<Product> GetProductByIdAsync(int id);
}