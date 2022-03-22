using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace product_crud_api.Infra.Models
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Lot { get; set; }
        [Precision(10,2)]
        public decimal Price { get; set; } 
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
