using eCommerceApp.Application.DTOs.Category;
using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Product
{
    public class GetProduct :Productbase
    {
        [Required]
        public int Id { get; set; }
        public GetCategory? Category { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
