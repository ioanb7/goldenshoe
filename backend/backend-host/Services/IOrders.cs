using System.Threading.Tasks;
using aspcore.Actors;
using Akka.Actor;
using Messages;

namespace aspcore.Services
{
    public interface IOrders
    {
        Task<object> CreateOrder(int productId, decimal price, string jwt);
        Task<object> GetOrderById(int orderId, string jwt);
        Task<object> GetAllOrders(string jwt);
        Task<object> ProcessOrder(int orderId, string cardId, int cardCode, decimal amount, string jwt);
        Task<object> CompleteOrder(int orderId);
        

    }

    public class Orders : IOrders
    {
        private ActorSelection _actor;

        public Orders(OrderActorProvider aap)
        {
            _actor = aap.Get();
        }

        public async Task<object> CreateOrder(int productId, decimal price, string jwt)
        {
            return await _actor.Ask(new Messages.Order.CreateTempOrderCommand()
            {
                ProductId = productId,
                Price = price,
                JWT = jwt
            });
        }

        public async Task<object> GetOrderById(int orderId, string jwt)
        {
            return await _actor.Ask(new Messages.Order.GetOrderByIdCommand()
            {
                OrderId = orderId,
                JWT = jwt
            });
        }

        public async Task<object> GetAllOrders(string jwt)
        {
            return await _actor.Ask(new Messages.Order.GetAllOrdersCommand()
            {
                JWT = jwt
            });
        }

        public async Task<object> ProcessOrder(int orderId, string cardId, int cardCode, decimal amount, string jwt)
        {
            return await _actor.Ask(new Messages.Order.ProcessOrderCommand()
            {
                OrderId = orderId,
                CardId = cardId,
                CardCode = cardCode,
                Amount = amount,
                JWT = jwt
            });
        }


        public async Task<object> CompleteOrder(int orderId)
        {
            return await _actor.Ask(new Messages.Order.CompleteOrderCommand()
            {
                OrderId = orderId
            });
        }

    }
}