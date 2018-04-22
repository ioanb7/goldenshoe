using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace authenticator
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



            string publicHostname = "authenticator-service";
            if (isDev) publicHostname = "localhost";


            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }
    remote {
        dot-netty.tcp {
            port = 9000
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



            using (var system = ActorSystem.Create("AuthenticatorServer", config))
            {
                system.ActorOf<AuthenticatorActor>("Authenticator");
                Console.WriteLine("ActorOf/");

                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
    public class AuthenticatorActor : ReceiveActor
    {
        public Dictionary<string, string> accounts = new Dictionary<string, string>();
        private IConfigurationRoot Configuration;

        public AuthenticatorActor()
        {
            Console.WriteLine("AuthenticatorActor");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            Receive<Messages.Authenticator.RegisterCommand>(cmd => Register(cmd));
            Receive<Messages.Authenticator.LoginCommand>(cmd => Login(cmd));
            Console.WriteLine("AuthenticatorActor/");
        }

        private void Register(Messages.Authenticator.RegisterCommand cmd)
        {
            Console.WriteLine("Register command");

            if (String.IsNullOrEmpty(cmd.Username) || cmd.Username.Length < 6)
            {
                var invalidData = new Messages.Authenticator.FormInvalidData("username");
                Sender.Tell(invalidData);
                return;
            }

            if (String.IsNullOrEmpty(cmd.Name) || cmd.Name.Length < 6)
            {
                Sender.Tell(new Messages.Authenticator.FormInvalidData("name"));
                return;
            }

            if (String.IsNullOrEmpty(cmd.Password) || cmd.Password.Length < 6)
            {
                Sender.Tell(new Messages.Authenticator.FormInvalidData("password"));
                return;
            }

            if (String.IsNullOrEmpty(cmd.Password2) || cmd.Password2 != cmd.Password)
            {
                Sender.Tell(new Messages.Authenticator.FormInvalidData("password != password2"));
                return;
            }

            using (MyContext context = MyContext.Connect(GetPath()))
            {
                if (context.Users.Any(u => u.Username == cmd.Username))
                {
                    Sender.Tell(new Messages.Authenticator.RegisterUsernameExists());
                }



                context.Users.Add(new User
                {
                    Name = cmd.Name,
                    Username = cmd.Username,
                    Password = cmd.Password,
                    //ApiKey = generateApiKey()
                });


                context.SaveChanges();


                Sender.Tell(new Messages.Authenticator.RegisterSuccess());
            }
        }

        private string generateApiKey()
        {
            return Guid.NewGuid().ToString();
        }

        private void Login(Messages.Authenticator.LoginCommand cmd)
        {
            Console.WriteLine("Login command");

            using (MyContext context = MyContext.Connect(GetPath()))
            {
                var result = context.Users.Where(p => p.Username == cmd.Username).ToList();

                if (result.Count > 0)
                {
                    var userDb = result[0];
                    if (userDb.Password == cmd.Password)
                    {

                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, userDb.Username),
                            new Claim("Name", userDb.Name),
                            new Claim("Id", userDb.Id.ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: "yourdomain.com",
                            audience: "yourdomain.com", // TODO: use Configuration file for this
                            claims: claims,
                            expires: DateTime.Now.AddDays(30),
                            signingCredentials: creds);
                        
                        
                        Sender.Tell(new Messages.Authenticator.LoginSuccess(new JwtSecurityTokenHandler().WriteToken(token)));
                    }
                    else
                    {
                        Sender.Tell(new Messages.Authenticator.LoginFailedPassword());
                    }
                }
            }
        }



        private string GetPath()
        {
            bool isDev = Environment.GetEnvironmentVariable("RUNNING_AS") == "Development";

            string database = "authenticator";

            string path = @"User Id=root;Host=database;Database=" + database + ";";
            if (isDev) path = @"User Id=root;Host=localhost;Database=" + database + ";";

            return path;
        }
    }
}
