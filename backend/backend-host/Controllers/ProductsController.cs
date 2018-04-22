using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Messages.Products;
using Products = Messages.Products;

namespace aspcore.Controllers
{
    [Produces("application/json")]
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        private IProducts products;

        public ProductsController(IProducts products)
        {
            this.products = products;
        }


        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<Messages.Products.Product>> Get()
        {
            var result = await products.GetAllProducts();

            if (result is Messages.Products.AllProducts)
            {
                Response.StatusCode = 200;

                var msg = (Messages.Products.AllProducts) result;
                return msg.Products;
            }

            return new List<Products.Product>();
        }


        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<Messages.Products.Product> Get(int id)
        {
            var result = await products.GetProductById(id);

            if (result is ProductResponse)
            {
                Response.StatusCode = 200;

                var msg = (ProductResponse)result;
                if (msg.IsSuccess == true)
                {
                    return msg.Product;
                }
                else
                {
                    return msg.Product;
                }
            }

            return null;
        }
    }
}