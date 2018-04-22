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
    [Route("api/Tracking")]
    public class TrackingController : Controller
    {
        private ITracking tracking;
        private IOrders orders;
        private IProducts products;

        public TrackingController(ITracking tracking, IOrders orders, IProducts products)
        {
            this.tracking = tracking;
            this.orders = orders;
            this.products = products;
        }
        
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            Console.WriteLine("TrackingController/Get/{id}");
            var result = await tracking.GetTrackingOrderForOrder(id, GetJwt());
            Console.WriteLine(result);
            if (result is Messages.Tracking.TrackingOrder)
            {
                return result;
            }
            if (result is Messages.ErrorOccured)
            {
                Response.StatusCode = 400;
                return result;
            }

            return result;

            //return null;
        }

        [HttpPut("{id}")]
        public async Task<object> Update([FromBody]Messages.Tracking.UpdateTrackingForOrderCommand cmd, int id)
        {
            var result = await tracking.UpdateTrackingOrder(id, cmd.CurrentLocation, cmd.EstimatedArrival, cmd.Progress, GetJwt());

            if (result is Messages.Tracking.TrackingOrderUpdateSuccess)
            {
                if (cmd.Progress == 100)
                {
                    var resultFinishedOrder = await orders.CompleteOrder(id);
                    Messages.Order.OrderDeliveredSuccess orderComplete =
                        (Order.OrderDeliveredSuccess) resultFinishedOrder;
                }
            }

            /*
            if (result is Messages.Tracking.TrackingOrderUpdateFail)
            {
                return result;
            }*

            return null;
            */
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