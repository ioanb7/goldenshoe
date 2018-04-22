using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrackingService
{
    public class TrackedOrder
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        public int OrderId { get; set; }
        public int Progress { get; set; } // 0 to 100
        public string CurrentLocation { get; set; } // warehouse
        public DateTime CreatedAt { get; set; }
        public DateTime EstimatedArrival { get; set; }
    }
}
