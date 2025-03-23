using System;
using System.Collections.Generic;

namespace TravelFoodCms.Models.DTOs
{
  public class OrderItemDTO
    {
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

}  