using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Product
{
    public  class UpdateProduct :Productbase
    {
        [Required]
        public int Id { get; set; }
       

    }
}
