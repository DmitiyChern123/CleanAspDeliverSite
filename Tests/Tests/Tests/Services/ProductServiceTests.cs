using DiplomApplication.Application.Interfaces;
using DiplomApplication.Domain.Entities;
using DiplomApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IWebHostEnvironment> _envMock = new();
    private  readonly CancellationToken cancellationToken = default;
    private ProductService CreateService() =>
        new(_productRepoMock.Object, _userRepoMock.Object, _unitOfWorkMock.Object, _envMock.Object);

    [Fact]
    public async Task LoadCategoryList_ShouldReturnCategories()
    {
        
        var expected = new List<Category> { new() { Name = "Coffee" } };
        _productRepoMock.Setup(x => x.GetCatalogAsync()).ReturnsAsync(expected);
        var service = CreateService();

        
        var result = await service.LoadCategoryList();

        
        Assert.Single(result);
        Assert.Equal("Coffee", result[0].Name);
    }[Fact]
    public async Task AddProductInCart_ShouldAddNewItem_WhenNotInCart()
    {
        var user = new User { Id = 1 };
        _userRepoMock.Setup(x => x.GetByLoginAsync("test")).ReturnsAsync(user);
        _productRepoMock.Setup(x => x.CheckCartAsync(100, 1)).ReturnsAsync(true);
        var service = CreateService();
        
        await service.AddProductInCart("test", 100, 5);
        
        _productRepoMock.Verify(x => x.AddAsync(It.Is<Cart>(c => c.UserId == 1 && c.ProductTypeId == 5)), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cancellationToken) , Times.Once);
    }

    [Fact]
    public async Task AddProductInCart_ShouldIncrementCount_WhenItemExists()
    {
        var user = new User { Id = 2 };
        _userRepoMock.Setup(x => x.GetByLoginAsync("test")).ReturnsAsync(user);
        _productRepoMock.Setup(x => x.CheckCartAsync(100, 2)).ReturnsAsync(false);
        var service = CreateService();
        
        await service.AddProductInCart("test", 100, 5);
        
        _productRepoMock.Verify(x => x.CartPlusCountAsync(100, 2, 1), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);
    }

}