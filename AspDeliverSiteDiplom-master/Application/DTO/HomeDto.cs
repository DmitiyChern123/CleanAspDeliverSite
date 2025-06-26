using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Application.DTO;

public class HomeDto
{
    public List<Product> Products { get; set; }
    public List<Category> Categories { get; set;}
    public List<ProductType> hasinkorzina { get; set; }
    public List<Cart>? CartInfo { get; set; }
    public int? SelectedCategoryId { get; set; }
    public string SortOrder { get; set; }
    public string SearchQuery { get; set; }
    public string UserName { get; set; }

}