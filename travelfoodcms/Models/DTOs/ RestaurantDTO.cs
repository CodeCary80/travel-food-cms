using System;
using System.Collections.Generic;

namespace TravelFoodCms.Models.DTOs
{
    public class RestaurantDTO
    {
        public int RestaurantId { get; set; }
        public int DestinationId { get; set; }
        public string Name { get; set; }
        public string CuisineType { get; set; }
        public string PriceRange { get; set; }
        public string ContactInfo { get; set; }
        public string OperatingHours { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Date { get; set; }
        public List<OrderDTO> Orders { get; set; }
    }

}  