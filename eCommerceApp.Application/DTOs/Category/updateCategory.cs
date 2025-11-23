using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Category
{
    public class UpdateCategory : CategoryBase
    {
        [Required]
        public int Id { get; set; }
    }
}
