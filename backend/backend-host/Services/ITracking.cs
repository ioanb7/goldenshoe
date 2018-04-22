using System;
using System.Threading.Tasks;
using aspcore.Actors;
using Akka.Actor;
using Messages;

namespace aspcore.Services
{
    public interface ITracking
    {
        Task<object> CreateTrackingOrder(int orderId, string currentLocation, DateTime estimatedArrival, string jwt);
        Task<object> UpdateTrackingOrder(int orderId, string currentLocation, DateTime? estimatedArrival, int progress, string jwt);
        Task<object> GetTrackingOrderForOrder(int orderId, string jwt);
    }

    public class Tracking : ITracking
    {
        private ActorSelection _actor;

        public Tracking(TrackingActorProvider aap)
        {
            _actor = aap.Get();
        }

        public async Task<object> CreateTrackingOrder(int orderId, string currentLocation, DateTime estimatedArrival, string jwt)
        {
            return await _actor.Ask(new Messages.Tracking.CreateTrackingOrderCommand()
            {
                OrderId = orderId,
                CurrentLocation = currentLocation,
                EstimatedArrival = estimatedArrival,
                JWT = jwt
            });
        }

        public async Task<object> UpdateTrackingOrder(int orderId, string currentLocation, DateTime? estimatedArrival, int progress, string jwt)
        {
            return await _actor.Ask(new Messages.Tracking.UpdateTrackingForOrderCommand()
            {
                OrderId = orderId,
                CurrentLocation = currentLocation,
                EstimatedArrival = estimatedArrival,
                Progress = progress,
                JWT = jwt
            });
        }

        public async Task<object> GetTrackingOrderForOrder(int orderId, string jwt)
        {
            return await _actor.Ask(new Messages.Tracking.GetTrackingForOrderCommand()
            {
                OrderId = orderId,
                JWT = jwt
            });
        }
    }
}