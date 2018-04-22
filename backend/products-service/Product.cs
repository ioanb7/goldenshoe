using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace products
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int ProductType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal ShoeSize { get; set; }

        public IEnumerable<ProductImage> Images { get; set; }

        public Product() { }
    }
}
