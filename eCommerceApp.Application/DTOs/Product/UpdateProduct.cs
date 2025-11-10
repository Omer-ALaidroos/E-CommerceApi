using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Product
{
    public  class UpdateProduct :Productbase
    {
        [Required]
        public Guid Id { get; set; }
       
    }
}
