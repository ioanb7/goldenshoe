using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aspcore.Actors;
using Akka.Actor;
using aspcore.Data;
using aspcore.Middlewares;
using aspcore.Models;
using aspcore.Services;
using Akka.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;

namespace aspcore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "yourdomain.com",
                        ValidAudience = "yourdomain.com",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))
                    };
                });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
            });
            services.AddMvc();
            services.AddSingleton<IBookRepository, BookRepository>();

            bool isDevOnHost = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "DevelopmentHost";

            if(isDevOnHost)
            {
                Console.WriteLine("Running in DEV mode on Host");
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseMySql(Configuration.GetConnectionString("DefaultConnectionDev")));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            }


            string thisHostname = "backend-host";
            if (isDevOnHost) thisHostname = "localhost";

            string authenticatorHostname = "authenticator-service";
            if (isDevOnHost) authenticatorHostname = "localhost";

            string productsHostname = "products-service";
            if (isDevOnHost) productsHostname = "localhost";

            string ordersHostname = "orders-service";
            if (isDevOnHost) ordersHostname = "localhost";

            string trackingHostname = "tracking-service";
            if (isDevOnHost) trackingHostname = "localhost";

            //authenticatorHostname = "192.168.99.100";

            //services.AddSingleton<IApplicationDbContext, ApplicationDbContext>();

            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }
    remote {
        dot-netty.tcp {
		    port = 0
		    hostname = "+thisHostname+@"
        }
    }
}
");

            services.AddSingleton<ActorSystem>(serviceProvider => ActorSystem.Create("aspcore-app", config));

            services.AddSingleton<IAuthenticator, Authenticator>();
            services.AddSingleton<AuthenticatorActorProvider>(serviceProvider =>
                new AuthenticatorActorProvider(
                    "akka.tcp://AuthenticatorServer@" + authenticatorHostname + ":9000/user/Authenticator",
                    serviceProvider.GetService<ActorSystem>()));

            services.AddSingleton<IProducts, Products>();
            services.AddSingleton<ProductsActorProvider>(serviceProvider =>
                new ProductsActorProvider(
                    "akka.tcp://ProductsServer@" + productsHostname + ":9001/user/Products",
                    serviceProvider.GetService<ActorSystem>()));

            services.AddSingleton<IOrders, Orders>();
            services.AddSingleton<OrderActorProvider>(serviceProvider =>
                new OrderActorProvider(
                    "akka.tcp://OrderServer@" + ordersHostname + ":9002/user/Order",
                    serviceProvider.GetService<ActorSystem>()));

            services.AddSingleton<ITracking, Tracking>();
            services.AddSingleton<TrackingActorProvider>(serviceProvider =>
                new TrackingActorProvider(
                    "akka.tcp://TrackingServer@" + trackingHostname + ":9003/user/Tracking",
                    serviceProvider.GetService<ActorSystem>()));




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext db)
        {
            app.UseAuthentication();

            /*
            app.UseCors(builder =>
                //builder.WithOrigins("http://192.168.99.100:80/", "http://192.168.99.100:3000/", "http://192.168.99.100", "http://localhost:3000", "http://127.0.0.1:3000")
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader());*/

            app.UseCors("AllowAllOrigins");


            if (env.IsDevelopment())
            {

                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            try
            {
                db.Database.Migrate();
            }
            catch(Exception e)
            {
                if (e.Message.Contains("Unable to connect to any of the specified "))
                {
                    Console.WriteLine(
                        "Error migrating the database, unable to connect.");
                    Environment.Exit(1);
                }
                else
                {
                    throw;
                }
            }

            app.UseGraphQl();

            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
