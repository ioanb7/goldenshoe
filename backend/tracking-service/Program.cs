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
using Microsoft.Extensions.Configuration;

namespace TrackingService
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
            
            string publicHostname = "tracking-service";
            if (isDev) publicHostname = "localhost";


            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }
    remote {
        dot-netty.tcp {
            port = 9003
            hostname = 0.0.0.0
            public-hostname = "+ publicHostname + @"
        }
    }
}
        ");
            
            using (var system = ActorSystem.Create("TrackingServer", config))
            {
                system.ActorOf<TrackingActor>("Tracking");

                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
    public class TrackingActor : ReceiveActor
    {
        public Dictionary<string, string> accounts = new Dictionary<string, string>();
        private IConfigurationRoot Configuration;

        public TrackingActor()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            Receive<Messages.Tracking.CreateTrackingOrderCommand>(cmd => CreateTrackingOrder(cmd));
            Receive<Messages.Tracking.GetTrackingForOrderCommand>(cmd => GetTrackingOrderForOrder(cmd));
            Receive<Messages.Tracking.UpdateTrackingForOrderCommand>(cmd => UpdateTrackingForOrder(cmd));
        }


        private void CreateTrackingOrder(Messages.Tracking.CreateTrackingOrderCommand cmd)
        {
            int userId = getUserIdFromJwt(cmd.JWT);
            if (userId == -1)
                Sender.Tell(new Messages.Unauthorised());

            using (MyContext context = MyContext.Connect(GetPath()))
            {
                TrackedOrder trackedOrder = new TrackedOrder()
                {
                    CreatedAt = DateTime.Now,
                    CurrentLocation = cmd.CurrentLocation,
                    EstimatedArrival = cmd.EstimatedArrival,
                    OrderId = cmd.OrderId,
                    Progress = 10, // processed
                    UserId = userId
                };

                context.TrackedOrders.Add(trackedOrder);
                context.SaveChanges();

                Sender.Tell(new Messages.Tracking.TrackingOrderCreateSuccess());
            }
        }

        private void GetTrackingOrderForOrder(Messages.Tracking.GetTrackingForOrderCommand cmd)
        {
            int userId = getUserIdFromJwt(cmd.JWT);
            if (userId < 1)
                Sender.Tell(new Messages.Unauthorised());

            using (MyContext context = MyContext.Connect(GetPath()))
            {
                var order = GetTrackingOrderForOrder(cmd.OrderId, context);
                if (order == null) // error from the past
                {
                    Sender.Tell(new Messages.ErrorOccured("An error has occured: the tracking data does not exist"));
                }
                if (order.UserId == userId)
                {
                    Sender.Tell(TrackingOrderDbToTrackingOrderMessage(order));
                }
                else
                {
                    Sender.Tell(new Unauthorised());
                }
            }
        }

        private Messages.Tracking.TrackingOrder TrackingOrderDbToTrackingOrderMessage(TrackedOrder trackedOrder)
        {
            return new Messages.Tracking.TrackingOrder
            {
                CreatedAt = trackedOrder.CreatedAt,
                CurrentLocation = trackedOrder.CurrentLocation,
                EstimatedArrival = trackedOrder.EstimatedArrival,
                Id = trackedOrder.Id,
                OrderId = trackedOrder.OrderId,
                Progress = trackedOrder.Progress,
                UserId = trackedOrder.UserId
            };
        }

        private TrackedOrder GetTrackingOrderForOrder(int orderId, MyContext context)
        {
            var result = context.TrackedOrders.Where(o => o.OrderId == orderId).ToList();

            if (result.Count > 0)
            {
                var orderDb = result[0];
                return orderDb;
            }

            return null;
        }
        

        private void UpdateTrackingForOrder(Messages.Tracking.UpdateTrackingForOrderCommand cmd)
        {
            int userId = getUserIdFromJwt(cmd.JWT);
            if (userId < 1)
                Sender.Tell(new Messages.Unauthorised());

            using (MyContext context = MyContext.Connect(GetPath()))
            {
                var trackedOrder = GetTrackingOrderForOrder(cmd.OrderId, context);
                if (trackedOrder == null)
                {
                    Sender.Tell(new Unauthorised()); // TODO: error TrackingOrderUpdateFail
                    return;
                }

                /*
                 //anyone can update this. TODO: check role, not the userId
                if (userId != trackedOrder.UserId)
                {
                    Sender.Tell(new Unauthorised());
                    return;
                }*/

                trackedOrder.CurrentLocation = cmd.CurrentLocation;
                trackedOrder.Progress = cmd.Progress;
                if (cmd.EstimatedArrival != null)
                {
                    trackedOrder.EstimatedArrival = cmd.EstimatedArrival ?? default(DateTime);
                }


                context.SaveChanges();

                Sender.Tell(new Messages.Tracking.TrackingOrderUpdateSuccess());
            }
        }
        



        private string GetPath()
        {
            bool isDev = Environment.GetEnvironmentVariable("RUNNING_AS") == "Development";

            string database = "tracking";

            string path = @"User Id=root;Host=database;Database=" + database + ";";
            if (isDev) path = @"User Id=root;Host=localhost;Database=" + database + ";";

            return path;
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
    }
}
