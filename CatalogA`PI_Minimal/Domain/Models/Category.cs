using System.Text.Json.Serialization;

namespace CatalogA_PI_Minimal.Domain.Models
{
    public class Category
    {
        public Category()
        {
            CategoryId = Guid.NewGuid();
            CreatedAt = DateTime.Now.Date;
            Products = new List<Product>();
        }
        public Guid CategoryId { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set ; }
        [JsonIgnore]
        public ICollection<Product> Products { get; set; }
    }
}
