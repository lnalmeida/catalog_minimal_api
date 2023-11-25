using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogA_PI_Minimal.Domain.Models
{
    public class Product
    {
        public Product() 
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now.Date;
        }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
