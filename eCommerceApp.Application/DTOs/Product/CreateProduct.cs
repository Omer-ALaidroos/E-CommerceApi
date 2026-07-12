using Microsoft.AspNetCore.Http;

namespace eCommerceApp.Application.DTOs.Product
{
    public class CreateProduct :Productbase
    {
      required  public int CategoryId { get; set; }
        public List<IFormFile> Images { get; set; } = [];
    }
}
