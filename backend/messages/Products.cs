using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messages
{
    public class Products
    {
        public enum ProductType
        {
            Shoe = 1
        }

        public class GetAllProductsCommand
        {
        }

        public class GetProductByIdCommand
        {
            public GetProductByIdCommand(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }

        public class ReserveProductCommand
        {
            public ReserveProductCommand(int productId, int orderId)
            {
                ProductId = productId;
                OrderId = orderId;
            }

            public int ProductId { get; }
            public int OrderId { get; }
        }

        public class ProductSoldCommand
        {
            public ProductSoldCommand(int productId, int orderId)
            {
                ProductId = productId;
                OrderId = orderId;
            }

            public int ProductId { get; }
            public int OrderId { get; }
        }


        public class GetSoonestOrderRetryCommand
        {
        }
        public class CreateProductCommand
        {
            public CreateProductCommand(Product product)
            {
                Product = product;
            }

            public Product Product { get; }
        }

        public class ProductImage
        {
            public ProductImage(int id, string title, string description, string filename)
            {
                Id = id;
                Title = title;
                Description = description;
                Filename = filename;
            }

            public int Id { get; }
            public string Title { get; }
            public string Description { get; }
            public string Filename { get; }
        }

        public class Product
        {
            public Product(int id, ProductType productType, string title, string description, decimal price, int stock, int reserved, IEnumerable<ProductImage> images, decimal shoeSize)
            {
                ProductType = productType;
                Id = id;
                Title = title;
                Description = description;
                Price = price;
                Stock = stock;
                Reserved = reserved;
                Images = images;
                ShoeSize = shoeSize;
            }

            public int Id { get; }
            public ProductType ProductType { get; }
            public string Title { get; }
            public string Description { get; }
            public decimal Price { get; }
            public int Stock { get; }
            public int Reserved { get; }
            public IEnumerable<ProductImage> Images { get; set; }
            public decimal ShoeSize { get; }
        }

        public class AllProducts
        {
            public AllProducts(IEnumerable<Product> products)
            {
                Products = products;
            }

            public IEnumerable<Product> Products { get; }
        }

        public class ProductSoonestRetry
        {
            public ProductSoonestRetry(DateTime soonestRetryDateTime)
            {
                SoonestRetryDateTime = soonestRetryDateTime;
            }

            public DateTime SoonestRetryDateTime { get; }
        }

        public class ProductResponse
        {
            public bool IsSuccess { get; set; }
            public Product Product { get; set; }
        }

        public class ProductResponseSuccess : ProductResponse
        {
            public ProductResponseSuccess(Product product)
            {
                IsSuccess = true;
                Product = product;
            }
        }
        public class ProductResponseError : ProductResponse
        {
            public ProductResponseError()
            {
                IsSuccess = false;
                Product = null;
            }
        }


        public class ProductReservedStatus : ResponseMessageWithStatus
        {
            public ProductReservedStatus(bool isSuccess) : base("Product reserved", isSuccess)
            {

            }
        }
    }
}
