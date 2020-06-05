using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NG_Core_Auth.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(150)]
        public string Description { get; set; }

        [Required]
        public bool OutofStock { get; set; }

        [Required]
        public string imageUrl { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
