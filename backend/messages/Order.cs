using System;
using System.Collections.Generic;

namespace Messages
{
    public class Order
    {
        public enum Status
        {
            Created = 1,
            Processing = 2,
            ProcessedSuccess = 3,
            ProcessedFail = 4,
            Delivered = 5
        }


        public class OrderObject
        {
            public int Id { get; set; }

            public int UserId { get; set; }
            public int ProductId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? FinishedAt { get; set; }
            public Status Status { get; set; }
            public decimal Price { get; set; }
        }

        public class OrderObjects
        {
            public IEnumerable<OrderObject> Orders { get; set; }
        }

        public class CreateTempOrderCommand : AuthenticatedMessage
        {
            public int ProductId { get; set; }
            public decimal Price { get; set; }
        }

        public class CompleteOrderCommand
        {
            public int OrderId { get; set; }
        }
        


        public class ProcessOrderCommand : AuthenticatedMessage
        {
            public int OrderId { get; set; }
            public string CardId { get; set; } // TODO: move these into user too.
            public int CardCode { get; set; }
            public decimal Amount { get; set; }
        }

        public class CheckOrderStatusCommand : AuthenticatedMessage
        {
            public int OrderId { get; set; }
        }

        public class GetOrderByIdCommand : AuthenticatedMessage
        {
            public int OrderId { get; set; }
        }
        public class GetAllOrdersCommand : AuthenticatedMessage
        {
        }

        



        public class TempOrderCreateSuccess : ResponseMessage
        {
            public TempOrderCreateSuccess(int orderId) : base("Order created")
            {
                OrderId = orderId;
            }

            public int OrderId { get; set; }
        }

        public class OrderStatusResponse
        {
            public int OrderId { get; set; }
            public Status Status { get; set; }
        }

        public class OrderDeliveredSuccess
        {
            public OrderObject Order { get; set; }
            public Status Status { get; set; }
        }

        

    }
}