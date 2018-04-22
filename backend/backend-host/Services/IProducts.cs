using System.Threading.Tasks;
using aspcore.Actors;
using Akka.Actor;
using Messages;

namespace aspcore.Services
{
    public interface IProducts
    {
        Task<object> GetAllProducts();
        Task<object> GetProductById(int id);
        Task<object> ReserveProduct(int productId, int orderId);
        Task<object> ProductSold(int productId, int orderId);
    }

    public class Products : IProducts
    {
        private ActorSelection _actor;

        public Products(ProductsActorProvider aap)
        {
            _actor = aap.Get();
        }

        public async Task<object> GetAllProducts()
        {
            return await _actor.Ask(new Messages.Products.GetAllProductsCommand());
        }

        public async Task<object> GetProductById(int id)
        {
            return await _actor.Ask(new Messages.Products.GetProductByIdCommand(id));
        }

        public async Task<object> ReserveProduct(int productId, int orderId)
        {
            return await _actor.Ask(new Messages.Products.ReserveProductCommand(productId, orderId));
        }

        public async Task<object> ProductSold(int productId, int orderId)
        {
            return await _actor.Ask(new Messages.Products.ProductSoldCommand(productId, orderId));
        }
    }
}