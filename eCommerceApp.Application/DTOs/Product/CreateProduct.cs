namespace eCommerceApp.Application.DTOs.Product
{
    public class CreateProduct :Productbase
    {
      required  public int CategoryId { get; set; }
    }
}
