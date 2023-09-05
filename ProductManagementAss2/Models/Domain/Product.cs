using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ProductManagementAss2.Models.Domain
{
    public class Product
    {
        [Key]
        public Guid ProdId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }

    }
}
