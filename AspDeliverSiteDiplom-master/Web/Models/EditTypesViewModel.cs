
using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Web.Models
{
    public class EditTypesViewModel
    {
        public List<ProductType>? types { get; set; }
        public ProductType ? newtype { get; set; }
        public int productid { get; set; }
    }
}
