using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Application.DTO;

public class EditTypesDto
{
    
    public List<ProductType>? types { get; set; }
    public ProductType ? newtype { get; set; }
    public int productid { get; set; }
}