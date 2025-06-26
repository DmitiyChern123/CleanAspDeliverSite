

using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Web.Models
{
    public class HomeModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set;}
        public List<ProductType> hasinkorzina { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string SortOrder { get; set; }
        public string SearchQuery { get; set; }
    }
}
