using System;

namespace Messages
{
    public class Tracking
    {
        public class TrackingOrder
        {
            public int Id { get; set; }
            public int UserId { get; set; }

            public int OrderId { get; set; }
            public int Progress { get; set; } // 0 to 100
            public string CurrentLocation { get; set; } // warehouse
            public DateTime CreatedAt { get; set; }
            public DateTime EstimatedArrival { get; set; }
        }

        public class CreateTrackingOrderCommand : AuthenticatedMessage
        {
            public int OrderId { get; set; }
            public DateTime EstimatedArrival { get; set; }
            public string CurrentLocation { get; set; }
        }

        public class GetTrackingForOrderCommand : AuthenticatedMessage
        {
            public int OrderId { get; set; }
        }

        public class UpdateTrackingForOrderCommand : AuthenticatedMessage
        {
            public int OrderId { get; set; }
            public DateTime? EstimatedArrival { get; set; }
            public string CurrentLocation { get; set; }
            public int Progress { get; set; } // 0 to 100
        }

        public class TrackingOrderCreateSuccess : ResponseMessageWithStatus
        {
            public TrackingOrderCreateSuccess() : base("Tracking for your order succeeded.", true)
            {
            }
        }
        public class TrackingOrderUpdateSuccess : ResponseMessageWithStatus
        {
            public TrackingOrderUpdateSuccess() : base("Tracking for your order updated - success.", true)
            {
            }
        }
        public class TrackingOrderUpdateFail : ResponseMessageWithStatus
        {
            public TrackingOrderUpdateFail(string reason) : base("Tracking for your order updated - failed: " + reason, false)
            {
            }
        }


    }
}