using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace products
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Filename { get; set; }

        public ProductImage() { }
    }
}
