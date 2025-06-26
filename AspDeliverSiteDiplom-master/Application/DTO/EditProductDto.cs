using System.ComponentModel.DataAnnotations;
using DiplomApplication.Application.clases;
using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Application.DTO;

public class EditProductDto
{
    
    [Required(ErrorMessage = "не верно заполнен продукт ")]
    public ValidateProduct? product { get; set; }

    public List<ProductType>? types { get; set; }

    public List<Category>? categories { get; set; }
      
    public int? selcat { get; set; }
       
    public IFormFile? ImgFile { get; set; }

}