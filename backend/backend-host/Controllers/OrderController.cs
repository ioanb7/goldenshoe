using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcore.Services;
using Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Messages.Products;
using Products = Messages.Products;

namespace aspcore.Controllers
{
    [Produces("application/json")]
    [Route("api/Order")]
    public class OrderController : Controller
    {
        private IOrders orders;
        private ITracking tracking;
        private IProducts products;

        public OrderController(IOrders orders, ITracking tracking, IProducts products)
        {
            this.orders = orders;
            this.tracking = tracking;
            this.products = products;
        }


        [HttpGet]
        public async Task<object> GetAllOrders()
        {
            var result = await orders.GetAllOrders(GetJwt());
            
            return result;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            var result = await orders.GetOrderById(id, GetJwt());

            if (result is Order.OrderObject)
            {
                return result;
            }

            return result;

            //return null;
        }

        public class PorcessOrderInput
        {
            public int OrderId { get; set; }
            public string CardId { get; set; }
            public int CardCode { get; set; }
        }

        [HttpPost("process")]
        public async Task<object> ProcessOrder([FromBody] PorcessOrderInput cmd)
        {
            var resultOrder = await orders.GetOrderById(cmd.OrderId, GetJwt());

            if (resultOrder is Order.OrderObject)
            {
                var resultOrderResponse = (Order.OrderObject)resultOrder;

                var resultProduct = await products.GetProductById(resultOrderResponse.ProductId);

                if (resultProduct is ProductResponse)
                {
                    var resultProductResponse = (ProductResponse) resultProduct;


                    var resultFinishedProduct = await products.ProductSold(resultOrderResponse.ProductId, cmd.OrderId);
                    //SUCCESS
                    return await orders.ProcessOrder(cmd.OrderId, cmd.CardId, cmd.CardCode, resultProductResponse.Product.Price, GetJwt());
                }

                return resultProduct;
            }
            return resultOrder;
        }

        [HttpPost]
        public async Task<object> CreateOrder([FromBody]Messages.Order.CreateTempOrderCommand cmd)
        {

            decimal price = ((ProductResponseSuccess) await products.GetProductById(cmd.ProductId)).Product.Price;


            Console.WriteLine("Order/CreateOrder");
            var result = await orders.CreateOrder(cmd.ProductId, price, GetJwt());
            Console.WriteLine("Order/CreateOrder /result");
            if (result is Messages.Order.TempOrderCreateSuccess)
            {
                Console.WriteLine("result is Success");
                //talk to the tracking service
                var resultCmd = (Messages.Order.TempOrderCreateSuccess)result;

                var reserveProductResult = await products.ReserveProduct(cmd.ProductId, resultCmd.OrderId);
                if ( !(reserveProductResult is ProductReservedStatus) ||
                     ((ProductReservedStatus)reserveProductResult).Status != true)
                {
                    //TODO: await orders.ForceRemoveOrder(resultCmd.OrderId);
                    Console.WriteLine("Order/CreateOrder Error Occured");
                    return reserveProductResult;
                }

                await tracking.CreateTrackingOrder(resultCmd.OrderId, "London", DateTime.Now + new TimeSpan(1, 2, 0, 0), GetJwt());

                Console.WriteLine("Order/CreateOrder return resultcmd");
                return resultCmd;
            }

            Console.WriteLine("Order/CreateOrder return result");
            return result;
        }

        private string GetJwt()
        {
            var jwt = Request.Headers["Authorization"];
            jwt = jwt.ToString().Substring("Bearer ".Length);
            return jwt;
        }
    }
}