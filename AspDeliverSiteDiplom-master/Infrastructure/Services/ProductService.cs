using DiplomApplication.Application.clases;
using DiplomApplication.Application.DTO;
using DiplomApplication.Application.Interfaces;
using DiplomApplication.Domain.Entities;
using Moq;

namespace DiplomApplication.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductService(IProductRepository productRepository, IUserRepository userRepository, IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }
   public async Task<List<Category>> LoadCategoryList()
    {
        return await _productRepository.GetCatalogAsync();
    }

    public async Task<HomeDto> LoadHomeData(HomeDto homeDto)
    {
        var user = await _userRepository.GetByLoginAsync(homeDto.UserName);
        List<Cart> addinp = new List<Cart>();
        if (user != null)
        {
            addinp = await _productRepository.GetCartsFromUserIdAsync(user.Id);
        }
        //var addinp = _context.Carts.Include(x => x.ProductType).Where(x => x.UserId == user.Id).ToList();
        List<ProductType> productsinkorzinas = new List<ProductType>();
        foreach (var item in addinp)
        {
            productsinkorzinas.Add(item.ProductType);
        }

        var categories = (await _productRepository.GetCategoriesAsync()).AsQueryable();
        var products = (await _productRepository.GetProductsAsync()).AsQueryable();
       
        if (homeDto.SelectedCategoryId != null)
        {
            if (homeDto.SelectedCategoryId != -1)
            {

                products = products.Where(p => p.IdCategory == homeDto.SelectedCategoryId.Value);
            }
        }
        if (!string.IsNullOrEmpty(homeDto.SearchQuery))
        {
            products = products.Where(p => p.Name.Contains(homeDto.SearchQuery));
        }
        switch (homeDto.SortOrder)
        {
            case "name_desc":
                products = products.OrderByDescending(p => p.Name);
                break;
            case "Price":
                products = products.OrderBy(p => p.Price);
                break;
            case "price_desc":
                products = products.OrderByDescending(p => p.Price);
                break;
            default:
                products = products.OrderBy(p => p.Name);
                break;
        }

        HomeDto newHomeDto = new HomeDto()
        {
            Categories = categories.ToList(),
            Products = products.ToList(),
            SelectedCategoryId = homeDto.SelectedCategoryId,
            SortOrder = homeDto.SortOrder,
            SearchQuery = homeDto.SearchQuery,
            hasinkorzina = productsinkorzinas,
            CartInfo = addinp
        };
        return newHomeDto;
    }

    public async Task AddProductInCart(string username, int productid, int propertyId)
    {
        var user = await _userRepository.GetByLoginAsync(username);
        if ( await _productRepository.CheckCartAsync(productid, user.Id))
        {
            Cart cart = new Cart()
            {
                ProductTypeId = propertyId,
                UserId = user.Id,
                Count = 1
            };
            _productRepository.AddAsync(cart);
            _unitOfWork.SaveChangesAsync();
        }
        else
        {
            _productRepository.CartPlusCountAsync(productid, user.Id, 1);
            _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<EditProductDto> GetProductInfo(int productId)
    {
        EditProductDto dto = new EditProductDto();
        dto.categories = await _productRepository.GetCatalogAsync();
        Product product;
        if (productId == 0)
        {
            product = new Product();
            product.Price = 0;
            var fileMock = new Mock<IFormFile>();
            
            // Настройка свойств заглушки файла
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.ContentType).Returns("text/plain");
            dto.ImgFile = fileMock.Object;
        }
        else
        {
            product = await _productRepository.GetProductByIdAsync(productId);
            dto.selcat = (int)product.IdCategory;
                
            using var stream = new FileStream("wwwroot/" + product.Img, FileMode.Open);
            IFormFile formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"  
            };
            dto.ImgFile = formFile;
            dto.types = product.ProductTypes.ToList();
        }
        dto.product = new ValidateProduct(product);
        return dto;
    }

    public async Task EditProduct(EditProductDto dto)
    {
        string uniqueFileName = "";
        if (dto.ImgFile != null && dto.ImgFile.Length > 0)
        {

            uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.ImgFile.FileName;



            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);


            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                dto.ImgFile.CopyToAsync(fileStream);
            }


        }

        if (dto.selcat != null)
        {

            if (dto.product.Id == 0)
            {
                Product product = new Product
                {
                    Name = dto.product.Name, Opis = dto.product.Opis, Img = "images/" + uniqueFileName,
                    Price = dto.product.Price, IdCategory = dto.selcat, IsHidden = dto.product.Is_hidden
                };
                ProductType productType = new ProductType
                    { Name = dto.product.Name, Price = (int)dto.product.Price };
                product.ProductTypes.Add(productType);
                _productRepository.AddAsync(product);
                //_context.Products.Add(product);
                //_context.SaveChanges();

            }
            else if (dto.product.Id > 0)
            {
                
                Product p = await _productRepository.GetProductByIdAsync(dto.product.Id);
                p.Name = dto.product.Name;
                p.Opis = dto.product.Opis;
                p.Price = dto.product.Price;
                p.IsHidden = dto.product.Is_hidden;
                if (dto.ImgFile != null)
                {

                    p.Img = "images/" + uniqueFileName;
                }

                p.IdCategory = dto.selcat;
                //_context.SaveChanges();
            }
        }

        _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<ProductType>> GetProductTypesById(int productid)
    {
        return (await _productRepository.GetProductByIdAsync(productid)).ProductTypes.ToList();
    }

    public async Task ChangeVisabilityProductType(int productTypeId, bool isDeleted)
    {
        ProductType productType = await _productRepository.GetProductTypeByIdAsync(productTypeId);
        if (productType != null)
        {
            productType.IsDelated = isDeleted;
            _productRepository.UpdateAsync(productType);
            _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task AddProducts(List<Product> products)
    {
            
            foreach (var product in products)
            {
                product.Id = 0;
                foreach (var productType in product.ProductTypes)
                {
                    productType.Id = 0;
                    productType.ProductId = 0;
                    
                }
                _productRepository.AddAsync(product);
            }
            products.Clear();
            _unitOfWork.SaveChangesAsync();
    }

    public async Task AddProductType(EditTypesDto dto)
    {
        var productType = new ProductType
        {
            Name = dto.newtype.Name,
            Price = dto.newtype.Price,
            ProductId = dto.productid,
            IsDelated = false
        };

        _productRepository.AddAsync(productType);
        _unitOfWork.SaveChangesAsync();
    }
}
