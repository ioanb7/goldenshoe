using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Messages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace OrdersService
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isDev = Environment.GetEnvironmentVariable("RUNNING_AS") == "Development";
            if (isDev)
            {
                Console.WriteLine("Running in Dev mode");
            }
            
            string publicHostname = "orders-service";
            if (isDev) publicHostname = "localhost";


            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }
    remote {
        dot-netty.tcp {
            port = 9002
            hostname = 0.0.0.0
            public-hostname = "+ publicHostname + @"
        }
    }
}
        ");
            
            using (var system = ActorSystem.Create("OrderServer", config))
            {
                system.ActorOf<OrderActor>("Order");

                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
    public class OrderActor : ReceiveActor
    {
        public Dictionary<string, string> accounts = new Dictionary<string, string>();
        private IConfigurationRoot Configuration;

        public OrderActor()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            Receive<Messages.Order.GetOrderByIdCommand>(cmd => GetOrderById(cmd));
            Receive<Messages.Order.GetAllOrdersCommand>(cmd => GetAllOrders(cmd));
            Receive<Messages.Order.CreateTempOrderCommand>(cmd => CreateTempOrder(cmd));
            Receive<Messages.Order.ProcessOrderCommand>(cmd => ProcessOrder(cmd));
            Receive<Messages.Order.CheckOrderStatusCommand>(cmd => CheckOrderStatus(cmd));
            Receive<Messages.Order.CompleteOrderCommand>(cmd => CompleteOrder(cmd));
            
        }

        private int getUserIdFromJwt(string token_)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            TokenValidationParameters validationParameters =
                new TokenValidationParameters
                {
                    ValidIssuer = "yourdomain.com",
                    ValidAudiences = new[] { "yourdomain.com" },
                    IssuerSigningKey = key
                };

            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            try
            {
                var decoded = handler.ValidateToken(token_, validationParameters, out validatedToken);

                foreach (var claim in decoded.Claims.ToList())
                {
                    if (claim.Type == "Id")
                    {
                        return int.Parse(claim.Value);
                    }
                }

            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException e)
            {

            }

            return -1;

        }

        
        private void GetAllOrders(Messages.Order.GetAllOrdersCommand cmd)
        {
            int userId = getUserIdFromJwt(cmd.JWT);
            if (userId < 1)
                Sender.Tell(new Messages.Unauthorised());

            using (MyContext context = MyContext.Connect(GetPath()))
            {
                Messages.Order.OrderObjects ordersResult = new Messages.Order.OrderObjects();
                var ordersList = new List<Messages.Order.OrderObject>();
                
                var result = context.Orders.Where(o => o.UserId == userId).ToList();
                foreach (var orderDb in result)
                {
                    ordersList.Add(OrderDbToOrderMessage(orderDb));
                }

                ordersResult.Orders = ordersList;
                Sender.Tell(ordersResult);
            }
        }

        private void GetOrderById(Messages.Order.GetOrderByIdCommand cmd)
        {
            int userId = getUserIdFromJwt(cmd.JWT);
            if (userId < 1)
                Sender.Tell(new Messages.Unauthorised());

            using (MyContext context = MyContext.Connect(GetPath()))
            {
                var order = GetOrderById(cmd.OrderId, context);
                if (order.UserId == userId)
                {
                    Sender.Tell(OrderDbToOrderMessage(order));
                }
                else
                {
                    Sender.Tell(new Unauthorised());
                }
            }
        }

        private void CheckOrderStatus(Messages.Order.CheckOrderStatusCommand cmd)
        {
            int userId = getUserIdFromJwt(cmd.JWT);
            if (userId < 1)
                Sender.Tell(new Messages.Unauthorised());

            using (MyContext context = MyContext.Connect(GetPath()))
            {
                var order = GetOrderById(cmd.OrderId, context);
                
                Sender.Tell(new Messages.Order.OrderStatusResponse
                {
                    OrderId = cmd.OrderId,
                    Status = (Messages.Order.Status)order.Status
                });
            }
        }

        private Messages.Order.OrderObject OrderDbToOrderMessage(Order order)
        {
            return new Messages.Order.OrderObject
            {
                CreatedAt = order.CreatedAt,
                FinishedAt = order.FinishedAt,
                Id = order.Id,
                ProductId = order.ProductId,
                Price = order.Price,
                Status = (Messages.Order.Status) order.Status
            };
        }

        private Order GetOrderById(int id, MyContext context)
        {
            var result = context.Orders.Where(o => o.Id == id).ToList();

            if (result.Count > 0)
            {
                var orderDb = result[0];
                return orderDb;
            }

            return null;
        }
        
        private void CreateTempOrder(Messages.Order.CreateTempOrderCommand cmd)
        {
            int userId = getUserIdFromJwt(cmd.JWT);
            if (userId == -1)
                Sender.Tell(new Messages.Unauthorised());

            using (MyContext context = MyContext.Connect(GetPath()))
            {
                Order order = new Order
                {
                    CreatedAt = DateTime.Now,
                    Status = Status.Created,
                    ProductId = cmd.ProductId,
                    Price = cmd.Price,
                    UserId = userId
                };

                context.Orders.Add(order);
                context.SaveChanges();

                Sender.Tell(new Messages.Order.TempOrderCreateSuccess(order.Id));
            }
        }

        private void ProcessOrder(Messages.Order.ProcessOrderCommand cmd)
        {
            int userId = getUserIdFromJwt(cmd.JWT);
            if (userId == -1)
                Sender.Tell(new Messages.Unauthorised());

            using (MyContext context = MyContext.Connect(GetPath()))
            {
                var order = GetOrderById(cmd.OrderId, context);
                if (order.Status == Status.Created)
                {
                    order.Status = Status.Processing;
                    context.SaveChanges();

                    //process the card here..
                    //bool processSuccess = await ProcessCard(cmd.CardId, cmd.CardCode, cmd.Amount);
                    bool processSuccess = ProcessCard(cmd.CardId, cmd.CardCode, cmd.Amount);
                    if (processSuccess)
                    {
                        order.Status = Status.ProcessedSuccess;
                        context.SaveChanges();
                    }
                    else
                    {
                        order.Status = Status.ProcessedFail;
                        context.SaveChanges();
                    }
                }

                Sender.Tell(new Messages.Order.OrderStatusResponse {
                    OrderId = order.Id,
                    Status = (Messages.Order.Status)order.Status});
            }
        }

        private void CompleteOrder(Messages.Order.CompleteOrderCommand cmd)
        {
            using (MyContext context = MyContext.Connect(GetPath()))
            {
                var order = GetOrderById(cmd.OrderId, context);
                if (order.Status == Status.ProcessedSuccess)
                {
                    order.Status = Status.Delivered;
                    order.FinishedAt = DateTime.Now;
                    context.SaveChanges();
                }

                Sender.Tell(new Messages.Order.OrderDeliveredSuccess { Order = OrderDbToOrderMessage(order), Status = (Messages.Order.Status)Messages.Order.Status.Processing });
            }
        }



        /*
        private async Task<bool> ProcessCard(string cardId, int cardCode, decimal amount)
        {
            return await Task.FromResult(true); // Suppose the card is valid
        }*/
        private bool ProcessCard(string cardId, int cardCode, decimal amount)
        {
            Thread.Sleep(3000);
            return true; // Suppose the card is valid
        }




        private string GetPath()
        {
            bool isDev = Environment.GetEnvironmentVariable("RUNNING_AS") == "Development";

            string database = "orders2";

            string path = @"User Id=root;Host=database;Database=" + database + ";";
            if (isDev) path = @"User Id=root;Host=localhost;Database=" + database + ";";

            return path;
        }
    }
}
