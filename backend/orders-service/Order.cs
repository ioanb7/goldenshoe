using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OrdersService
{
    public enum Status
    {
        Created = 1,
        Processing = 2,
        ProcessedSuccess = 3,
        ProcessedFail = 4,
        Delivered = 5
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public Status Status { get; set; }
        public decimal Price { get; set; }
    }
}
