using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceApp.Application.DTOs.Category
{
    public class CategoryBase
    {
        [Required]
        public string? Name { get; set; }
    }

   

  
}
