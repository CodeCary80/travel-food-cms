using System;
using System.Collections.Generic;

namespace TravelFoodCms.Models.DTOs
{
   public class OrderDTO
    {
        public int OrderId { get; set; }
        public int RestaurantId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string SpecialRequests { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }

}  