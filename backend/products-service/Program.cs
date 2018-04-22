using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using Messages;

namespace products
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

            string publicHostname = "products-service";
            if (isDev) publicHostname = "localhost";


            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }
    remote {
        dot-netty.tcp {
            port = 9001
            hostname = 0.0.0.0
            public-hostname = "+ publicHostname + @"
        }
    }
}
        ");

            /*
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<MyContext,
                    DataLayer.Migrations.Configuration>());
            
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<User>());

            */
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<MyContext, MyObjextContextMigration>());
            //MyObjectContext contexttest = new MyObjectContext();

            //var CodeGenerator = new MySqlMigrationCodeGenerator();



            using (var system = ActorSystem.Create("ProductsServer", config))
            {
                system.ActorOf<ProductsActor>("Products");

                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }

    public class ProductsActor : ReceiveActor
    {
        class Reservation
        {
            public int OrderId { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // TODO: delete expired List entries
        // TODO: save these in a database
        Dictionary<int, List<Reservation>> reservations = new Dictionary<int, List<Reservation>>();

        public ProductsActor()
        {
            Receive<Messages.Products.GetAllProductsCommand>(cmd => GetAllProducts());
            Receive<Messages.Products.GetProductByIdCommand>(cmd => GetProductById(cmd));
            Receive<Messages.Products.ProductSoldCommand>(cmd => ProductSoldCommand(cmd));
            Receive<Messages.Products.ReserveProductCommand>(cmd => ReserveProduct(cmd));
            
        }

        private void ReserveProduct(Products.ReserveProductCommand cmd)
        {
            if (!reservations.ContainsKey(cmd.ProductId))
            {
                reservations.Add(cmd.ProductId, new List<Reservation>());
            }
            reservations[cmd.ProductId].Add(new Reservation()
            {
                CreatedAt = DateTime.Now,
                OrderId = cmd.OrderId
            });

            Sender.Tell(new Messages.Products.ProductReservedStatus(true));
        }

        private void ProductSoldCommand(Products.ProductSoldCommand cmd)
        {
            using (MyContext context = MyContext.Connect(GetPath()))
            {
                Product product = GetProductById(cmd.ProductId, context);
                product.Stock--;
                context.SaveChanges();
            }

            if (!reservations.ContainsKey(cmd.ProductId))
            {
                Sender.Tell(new Messages.ResponseOk());
                return;
            }

            /*Delete it
            foreach (var productId in reservations.Keys)
            {
                for (int i = reservations.Count - 1; i >= 0; i--)
                {
                    if (reservations.Count == 0) continue;
                    if (reservations[productId][i].OrderId == cmd.OrderId)
                    {
                        //remove it
                        reservations[productId].RemoveAt(i);

                        Sender.Tell(new Messages.ResponseOk());
                    }
                }
            }*/

            // Invalidate it
            foreach (var productId in reservations.Keys)
            {
                if (reservations.Count == 0) continue;

                for (int i = reservations.Count - 1; i >= 0; i--)
                {
                    if (reservations[productId][i].OrderId == cmd.OrderId)
                    {
                        reservations[productId][i].CreatedAt = DateTime.MinValue;

                        Sender.Tell(new Messages.ResponseOk());
                    }
                }
            }


        }

        private int GetReservationsForProduct(int productId)
        {
            if (!reservations.ContainsKey(productId)) return 0;

            int result = 0;
            foreach (var dt in reservations[productId])
            {
                if (dt.CreatedAt + new TimeSpan(0, 0, 20, 0) > DateTime.Now)
                {
                    result++;
                }
            }

            return result;
        }

        private void GetProductById(Products.GetProductByIdCommand cmd)
        {
            using (MyContext context = MyContext.Connect(GetPath()))
            {
                Product product = GetProductById(cmd.Id, context);
                if (product != null)
                {
                    var productImages = getImagesForProduct(context, product.Id);
                    Sender.Tell(new Products.ProductResponseSuccess(fromDbToMessage(product,
                        GetReservationsForProduct(product.Id), productImages)));
                }
            }

            Sender.Tell(new Products.ProductResponseError());
        }

        private Product GetProductById(int id, MyContext context)
        {
            var result = context.Products.Where(p => p.Id == id).ToList();
            if (result.Count > 0)
            {
                return result[0];
            }

            return null;
        }


        private void GetAllProducts()
        {
            using (MyContext context = MyContext.Connect(GetPath()))
                //using (MyContext context = new MyContext(@"User Id=root;Host=localhost;Database=mydatabase;"))
                //using (MyContext context = new MyContext("MyContext"))
            {
                //context.Database.Initialize(true);

                List<Products.Product> products = new List<Products.Product>();

                var productsFromDb = context.Products.ToList();
                foreach (var productDb in productsFromDb)
                {
                    var productImages = getImagesForProduct(context, productDb.Id);

                    products.Add(fromDbToMessage(productDb, GetReservationsForProduct(productDb.Id), productImages));
                }

                Messages.Products.AllProducts allProducts = new Products.AllProducts(products);

                Sender.Tell(allProducts);
            }
        }



        private IEnumerable<Products.ProductImage> getImagesForProduct(MyContext context, int id)
        {
            var productImagesFromDb = context.ProductImages.Where(p => p.ProductId == id).ToList();

            List<Products.ProductImage> productImages = new List<Products.ProductImage>();
            foreach (var productImageDb in productImagesFromDb)
            {
                productImages.Add(new Products.ProductImage(
                    productImageDb.Id, productImageDb.Title, productImageDb.Description, productImageDb.Filename));
            }

            return productImages;
        }

        Products.Product fromDbToMessage(Product product, int reserved, IEnumerable<Products.ProductImage> productImages)
        {
            return new Products.Product(
                product.Id, (Products.ProductType)product.ProductType, product.Title, product.Description,
                product.Price, product.Stock, reserved, productImages, product.ShoeSize);
        }

        private string GetPath()
        {
            bool isDev = Environment.GetEnvironmentVariable("RUNNING_AS") == "Development";

            string database = "products";

            string path = @"User Id=root;Host=database;Database=" + database + ";";
            if (isDev) path = @"User Id=root;Host=localhost;Database=" + database + ";";

            return path;
        }
    }
}
