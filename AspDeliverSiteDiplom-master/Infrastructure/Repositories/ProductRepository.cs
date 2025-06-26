using DiplomApplication.Application.Interfaces;
using DiplomApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiplomApplication.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetCatalogAsync()
        => _context.Categories.Include(c=>c.Products).ToList();
   public async Task AddAsync(Product product) 
       => await _context.Products.AddAsync(product);
   public async Task UpdateAsync(Product product) 
       =>  _context.Products.Update(product);
   public async Task DeleteAsync(Product product)
       => _context.Products.Remove(product);
   
   public async Task AddAsync(Cart cart) 
       => await _context.Carts.AddAsync(cart);
   public async Task UpdateAsync(Cart cart) 
       =>  _context.Carts.Update(cart);
   public async Task DeleteAsync(Cart cart)
       => _context.Carts.Remove(cart);
   
   public async Task AddAsync(ProductType productType) 
       => await _context.ProductTypes. AddAsync(productType);
   public async Task UpdateAsync(ProductType productType) 
       =>  _context.ProductTypes.Update(productType);
   public async Task DeleteAsync(ProductType productType)
       => _context.ProductTypes.Remove(productType);

   public async Task<ProductType> GetProductTypeByIdAsync(int id)
       =>  await _context.ProductTypes.FirstOrDefaultAsync(x => x.Id == id);
   
   public async Task<List<Cart>> GetCartsFromUserIdAsync(int userid)
       => await _context.Carts.Include(x => x.ProductType).Where(x => x.UserId == userid).ToListAsync();

   public async Task<List<Category>> GetCategoriesAsync()
       => _context.Categories.Include(d => d.Products).ToList();

   public async Task<List<Product>> GetProductsAsync()
       => await _context.Products.Include(d => d.ProductTypes).Include(d => d.IdCategoryNavigation).ToListAsync();

   public async Task<bool> CheckCartAsync(int ProductTypeId, int UserID)
       => (await _context.Carts.FirstOrDefaultAsync(d => d.ProductTypeId == ProductTypeId && d.UserId == UserID)) is null;

   public async Task CartPlusCountAsync(int ProductTypeId, int UserID, int count)
   {
      Cart cart = await _context.Carts.FirstOrDefaultAsync(d => d.ProductTypeId == ProductTypeId && d.UserId == UserID);
      if (cart != null)
      {
          cart.Count = +count;
          _context.Carts.Update(cart);
      }
   }

   public async Task<List<ProductType>> GetProductTypesAsync()
       => await _context.ProductTypes.ToListAsync();

   public async Task UpdateQuantityCart(int id, int newCount)
   {
           var cart = _context.Carts.FirstOrDefault(a=>a.Id == id);
           if (cart!=null)
           {
               cart.Count = newCount;
           }
       
   }
   public async Task<Product> GetProductByIdAsync(int id) 
       => await _context.Products.Include(d => d.ProductTypes).FirstOrDefaultAsync(d => d.Id == id);
   
       

}